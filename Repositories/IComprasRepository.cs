using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface IComprasRepository
    {
        Task<IEnumerable<Compra>> GetAll();
        Task<Compra> GetId(int id);
        Task<bool> ExistID(int id);
        Task<bool> UsuarioActivoAsync(int idUsuario);
        Task<bool> ComprovanteExiste(string comprovante);
        Task<bool> ProveedorActivoAsync(int id);
        Task<bool> AlmacenActivoAsync(int id);
        Task<bool> ProductoActivoAsync(int id);
        Task Insert(Compra compra);
        Task<Compra?> GetCompraConDetallesAsync(int id);
        Task<Inventario?> GetInventarioAsync(int idProducto, int idAlmacen);
        Task SaveChangesAsync();
    }
}
