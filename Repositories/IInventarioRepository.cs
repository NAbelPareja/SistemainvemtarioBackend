using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface IInventarioRepository
    {
        Task<IEnumerable<Inventario>> GetAllInventariosAsync();
        Task<Inventario> GetInventarioAsync(int id);
        Task<Inventario?> ExistInventarioAsync(int id);
        Task Update();

    }
}
