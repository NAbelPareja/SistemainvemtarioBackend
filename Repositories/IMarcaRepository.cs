using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface IMarcaRepository
    {
        Task<IEnumerable<Marca>> GetAllAsync();
        Task<Marca> GetMarcaId(int id);
        Task<bool?> NameExistAsync(string name);
        Task<Marca?> MarcaExistAsync(int id);
        Task AddAsync(Marca marca);
        Task UpdateAsync();
        Task DeleteAsync(Marca marca);
    }
}
