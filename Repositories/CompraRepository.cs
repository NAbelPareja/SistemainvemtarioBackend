using Microsoft.EntityFrameworkCore;
using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public class CompraRepository: IComprasRepository
    {
        private readonly SistemaInventarioContext _context;
        public CompraRepository(SistemaInventarioContext context)
        {
            _context =  context;
        }

        public async Task<IEnumerable<Compra>> GetAll()
        {
            return await _context.Compras   
            .Include(c => c.DetalleCompras)
                .ThenInclude(d => d.IdProductoNavigation) // Jala los datos del producto comprado
            .Include(c => c.DetalleCompras)
                .ThenInclude(d => d.IdAlmacenNavigation)  // Jala los datos del almacén de destino
            .ToListAsync();
        }
        public async Task<Compra> GetId(int id)
        {
            return await _context.Compras
            .Include(c => c.DetalleCompras)
                .ThenInclude(d => d.IdProductoNavigation)
            .Include(c => c.DetalleCompras)
                .ThenInclude(d => d.IdAlmacenNavigation)
            .FirstOrDefaultAsync(d => d.IdCompra == id);
        }

        public async Task<bool> ExistID(int id)
        {
            return await _context.Compras.AnyAsync(d => d.IdCompra == id);
        }

        public async Task<bool> ComprovanteExiste(string comprovante)
        {
            return await _context.Compras.AnyAsync(d => d.ComprobanteReferencia != null &&  d.ComprobanteReferencia.ToLower() == comprovante.ToLower());
        }

        public async Task Insert(Compra compra)
        {
            // 1. Iniciamos una transacción "Todo o Nada" para asegurar consistencia
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 2. Guardamos la Compra y sus DetalleCompras en bloque
                _context.Compras.Add(compra);
                await _context.SaveChangesAsync(); // Aquí SQL Server genera el IdCompra y IdDetalleCompra automáticos

                // 3. Recorremos cada producto del detalle para actualizar la logística
                foreach (var detalle in compra.DetalleCompras)
                {
                    // Buscamos el casillero en Inventario para este Producto en este Almacén específico
                    var inventario = await _context.Inventarios
                        .FirstOrDefaultAsync(i => i.IdProducto == detalle.IdProducto && i.IdAlmacen == detalle.IdAlmacen);

                    if (inventario != null)
                    {
                        // ¡Hacemos la magia logístca! Sumamos la cantidad comprada al stock actual
                        inventario.Stock += detalle.Cantidad;
                        inventario.UltimaActualizacion = DateTime.Now;
                    }
                    else
                    {
                        // Por seguridad, si el casillero no existía, lo creamos (aunque ya lo automatizamos en AddProducto)
                        var nuevoInventario = new Inventario
                        {
                            IdProducto = detalle.IdProducto,
                            IdAlmacen = detalle.IdAlmacen,
                            Stock = detalle.Cantidad,
                            StockMinimo = 10,
                            UltimaActualizacion = DateTime.Now
                        };
                        await _context.Inventarios.AddAsync(nuevoInventario);
                    }

                    // 4. Registramos el movimiento lineal en el Kardex (Auditoría)
                    var movimiento = new MovimientoInventario
                    {
                        IdProducto = detalle.IdProducto,
                        IdAlmacen = detalle.IdAlmacen,
                        TipoMovimiento = "ENTRADA_COMPRA", // Identificador de tipo
                        Cantidad = detalle.Cantidad,
                        FechaMovimiento = DateTime.Now,
                        IdUsuario = compra.IdUsuarioRegistro, // El empleado que registró la factura
                        Referencia = $"Compra Ref: {compra.ComprobanteReferencia}"
                    };
                    await _context.MovimientoInventarios.AddAsync(movimiento);
                }

                // 5. Guardamos en SQL Server los cambios de Stock y Kardex
                await _context.SaveChangesAsync();

                // Si no hubo ningún error en el proceso, confirmamos la transacción permanentemente
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Si algo falla, se borra el borrador para que la base de datos quede intacta y limpia
                await transaction.RollbackAsync();
                throw; // Relanzamos el error para que llegue al controlador
            }
        }



    }
}
