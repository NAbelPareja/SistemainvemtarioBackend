using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface IComprasRepository
    {
        Task<IEnumerable<Compra>> GetAll();
        Task<Compra> GetId(int id);
        Task<bool> ExistID(int id);
        Task<bool> ComprovanteExiste(string comprovante);
        Task Insert(Compra compra);
    }
}
