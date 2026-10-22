using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface IVentaRepository
    {
        Task<IEnumerable<Ventum>> GetAll();
        Task<Ventum> GetId(int id);
        Task<bool> ExistId(int id);
        Task<Ventum> VentaExistasync(int id);
        Task<Inventario> ObetenerInventario(DetalleVentum detalle);
        Task<bool> AlmacenActivoAsync(int id);
        Task<bool> ProductoActivoAsync(int id);
        Task<Inventario?> InventarioExist(int idAlamcen, int idProducto);
        Task SaveChangesAsync();
        Task<bool> UsuarioActivoAsync(int idUsuario);
        Task Insert(Ventum venta);
    }
}
