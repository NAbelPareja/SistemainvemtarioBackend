using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface IProductoRepository
    {
        Task<List<Producto>> GetAllProductosAsync();
        Task<Producto> GetProductoAsync(int id);
        Task<Producto?> ExistProductoAsync(int id);
        Task<Categorium?> ExistCategoriaAsync(int id);
        Task<Marca?> ExistMarcaAsync(int id);
        Task<bool> ExistName(string name);
        Task AddProductoAsync(Producto producto);
        Task UpdateProductoAsync(Producto producto);
        Task DeleteProductoAsync(Producto producto);
    }
}
