using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface IAlmacenRepository
    {
        Task<IEnumerable<Almacen>> GetAll();
        Task<Almacen> GetAlmacenId(int id);
        Task<bool> NameExistAsync(string name);
        Task<Almacen?> AlmacenExistAsync(int id);
        Task AddAsync(Almacen almacen);
        Task UpdateAsync(Almacen almacen);
        Task DeleteAsync(Almacen almacen);
    }
}
