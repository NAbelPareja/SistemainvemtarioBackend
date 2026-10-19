using Microsoft.EntityFrameworkCore;
using SistemaInventario.Models;
using System.Collections;

namespace SistemaInventario.Repositories
{
    public class VentaRepository: IVentaRepository
    {
        private readonly SistemaInventarioContext _context;
        public VentaRepository(SistemaInventarioContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ventum>> GetAll()
        {
            return await _context.Venta
                 .Include(v => v.IdUsuarioRegistroNavigation) // SÍ: Quién vendió (Cajero)
        .Include(v => v.DetalleVenta)           // SÍ: La lista de productos vendidos
            .ThenInclude(d => d.IdProductoNavigation) // SÍ: Nombre del abarrote
        .Include(v => v.DetalleVenta)
            .ThenInclude(d => d.IdAlmacenNavigation)  // SÍ: De qué almacén salió
        .ToListAsync();
        }

        public async Task<Ventum> GetId(int id)
        {
            return await _context.Venta
                .Include(d => d.DetalleVenta)
                    .ThenInclude(d => d.IdProductoNavigation)
                .Include(d => d.DetalleVenta)
                    .ThenInclude(d => d.IdAlmacenNavigation)
                .FirstOrDefaultAsync(d => d.IdVenta == id);
        }

        public async Task<bool> ExistId(int id)
        {
            return await _context.Venta.AnyAsync(v=>v.IdVenta == id);
        }

        public async Task Insert(Ventum venta)
        {
            // 1. Iniciamos una transacción "Todo o Nada" para asegurar consistencia
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // PASO A: Validar primero que haya stock suficiente para CADA producto antes de guardar
                foreach (var detalle in venta.DetalleVenta)
                {
                    var inventario = await _context.Inventarios
                        .FirstOrDefaultAsync(i => i.IdProducto == detalle.IdProducto && i.IdAlmacen == detalle.IdAlmacen);

                    // Si el casillero no existe o la cantidad que pide el cliente es mayor a lo que hay en estantería
                    if (inventario == null || inventario.Stock < detalle.Cantidad)
                    {
                        // Conseguimos el nombre del producto para dar un error amigable al cajero
                        var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                        string nombreProd = producto != null ? producto.Nombre : $"ID: {detalle.IdProducto}";

                        // Detiene toda la operación inmediatamente y salta al catch sin romper la base de datos
                        throw new Exception($"Stock insuficiente para el producto '{nombreProd}'. Stock disponible actual: {(inventario != null ? inventario.Stock : 0)} unidades.");
                    }
                }

                // PASO B: Si todas las aduanas de stock pasaron con éxito, guardamos la Venta y sus Detalles
                _context.Venta.Add(venta);
                await _context.SaveChangesAsync(); // SQL Server genera los IDs correlativos de las boletas

                // PASO C: Procesamos la salida física de mercadería y registramos la huella en el Kardex
                foreach (var detalle in venta.DetalleVenta)
                {
                    // Volvemos a apuntar al inventario (ya sabemos con certeza que existe y tiene stock)
                    var inventario = await _context.Inventarios
                        .FirstAsync(i => i.IdProducto == detalle.IdProducto && i.IdAlmacen == detalle.IdAlmacen);

                    // !OPERACIÓN LOGÍSTICA OPUESTA A COMPRAS: Restamos del stock actual!
                    inventario.Stock -= detalle.Cantidad;
                    inventario.UltimaActualizacion = DateTime.Now;

                    // PASO D: Insertar fila en tu tabla de MovimientoInventario (Tu Kardex Contable)
                    var movimientoKardex = new MovimientoInventario
                    {
                        IdProducto = detalle.IdProducto,
                        IdAlmacen = detalle.IdAlmacen,
                        TipoMovimiento = "SALIDA_VENTA", // Etiqueta estricta de egreso por caja
                        Cantidad = detalle.Cantidad,
                        FechaMovimiento = DateTime.Now,
                        IdUsuario = venta.IdUsuarioRegistro, // El cajero que procesó el cobro
                        Referencia = $"Boleta de Venta Nro: {venta.IdVenta}"
                    };
                    await _context.MovimientoInventarios.AddAsync(movimientoKardex);
                }

                // Guardamos los cambios definitivos de la resta de stock y las huellas del Kardex en SQL Server
                await _context.SaveChangesAsync();

                // Confirmamos la transacción de forma permanente
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Si faltó stock en un fideo o una leche, o hubo un error de red, deshace todo el borrador
                await transaction.RollbackAsync();
                throw; // Lanza el mensaje exacto de excepción para que lo ataje tu controlador
            }
        }


    }
}
