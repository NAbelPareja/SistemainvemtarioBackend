using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface IVentaRepository
    {
        Task<IEnumerable<Ventum>> GetAll();
        Task<Ventum> GetId(int id);
        Task<bool> ExistId(int id);
        Task Insert(Ventum venta);
    }
}
