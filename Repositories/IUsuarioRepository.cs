using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAll();
        Task<Usuario> GetUsuarioID(int id);
        Task<bool> NameExistAsync(string name);
        Task<Usuario?> UsuarioExistAsync(int id);
        Task<Usuario?> ExistUsauarioAsync(string correo);
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(Usuario usuario);
    }
}
