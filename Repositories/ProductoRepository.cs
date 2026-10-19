using Microsoft.EntityFrameworkCore;
using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public class ProductoRepository: IProductoRepository
    {
        private readonly SistemaInventarioContext _context;
        public ProductoRepository(SistemaInventarioContext context)
        {
            _context = context;
        }
        public async Task<List<Producto>> GetAllProductosAsync()
        {
            return await _context.Productos.Include(m => m.IdMarcaNavigation).Include(c => c.IdCategoriaNavigation).ToListAsync();
        }

        public async Task<Producto> GetProductoAsync(int id)
        {
            return await _context.Productos
            .Include(m => m.IdMarcaNavigation)
            .Include(c => c.IdCategoriaNavigation)
            .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        public async Task<Producto?> ExistProductoAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }
        public async Task<Categorium?> ExistCategoriaAsync(int id)
        {
            return await _context.Categoria.FindAsync(id);
        }
        public async Task<Marca?> ExistMarcaAsync(int id)
        {
            return await _context.Marcas.FindAsync(id);
        }

        public async Task<bool> ExistName(string name)
        {
            return await _context.Productos.AnyAsync(p => p.Nombre == name);
        }

        public async Task AddProductoAsync(Producto producto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Esto es lo que ya tenías: Guardar el producto base
                _context.Productos.Add(producto);
                await _context.SaveChangesAsync(); // Aquí SQL Server genera el IdProducto automático

                // 2. Traer todos los almacenes activos del sistema
                var almacenes = await _context.Almacens.Where(a => a.Estado == true).ToListAsync();

                // 3. Crear automáticamente el casillero de inventario en cero para cada almacén
                foreach (var almacen in almacenes)
                {
                    var inventarioInicial = new Inventario
                    {
                        IdProducto = producto.IdProducto, // El ID generado automáticamente en el paso 1
                        IdAlmacen = almacen.IdAlmacen,
                        Stock = 0,         // Nace vacío hasta que registre una compra real
                        StockMinimo = 10,  // Tu alerta de stock mínimo por defecto para los abarrotes
                        UltimaActualizacion = DateTime.Now
                    };
                    await _context.Inventarios.AddAsync(inventarioInicial);
                }

                // 4. Guardar los registros de inventario en la base de datos
                await _context.SaveChangesAsync();

                // Si todo salió bien, confirma la transacción en SQL Server
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Si ocurre un error en el bucle o en la BD, revierte todo para no dejar datos corruptos
                await transaction.RollbackAsync();
                throw; // Relanza el error para que el controlador lo capture
            }
        }

        public async Task UpdateProductoAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteProductoAsync(Producto producto)
        {
            producto.Estado = false;
            _context.Productos.Update(producto);
             await _context.SaveChangesAsync();

        }
    }
}
