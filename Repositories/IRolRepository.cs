
using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface IRolRepository
    {
        Task<IEnumerable<Rol>> GetAllAsync();
        Task<bool?> NameExistasync(string name);
        Task<Rol?> RolexistAsync(int id);
        Task AddAsync(Rol rol);
        Task UpdateAsync(Rol rol);
        Task DeleteAsync(Rol rol);
    }
}
