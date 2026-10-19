using Microsoft.EntityFrameworkCore;
using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public class UsuarioRepository: IUsuarioRepository
    {
        private readonly SistemaInventarioContext _context;

        public UsuarioRepository(SistemaInventarioContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAll()
        {
            return await _context.Usuarios.Include(b=> b.IdRolNavigation).ToListAsync();
        }
        public async Task<Usuario> GetUsuarioID(int id)
        {
            return await _context.Usuarios.Include(b => b.IdRolNavigation).FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task<bool> NameExistAsync(string name)
        {
            return await _context.Usuarios.AnyAsync(u => u.Nombre == name);
        }

        public async Task<Usuario?> UsuarioExistAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> ExistUsauarioAsync(string correo)
        {
            return await _context.Usuarios.Include(r => r.IdRolNavigation).FirstOrDefaultAsync(u => u.Correo == correo);
        }

        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Usuario usuario)
        {
            usuario.Estado = false;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
