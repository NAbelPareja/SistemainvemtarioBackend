using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categorium>> GetAll();
        Task<Categorium> GetCategoriaId(int id);
        Task<bool?> NameExistAsync(string name);
        Task<Categorium?> CategoriaExistAsync(int id);
        Task AddAsync(Categorium categoria);
        Task UpdateAsync(Categorium categoria);
        Task DeleteAsync(Categorium categoria);
    }
}
