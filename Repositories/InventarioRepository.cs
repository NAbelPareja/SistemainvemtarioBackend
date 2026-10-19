using Microsoft.EntityFrameworkCore;
using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public class InventarioRepository: IInventarioRepository
    {
        private readonly SistemaInventarioContext _context;
        public InventarioRepository(SistemaInventarioContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Inventario>> GetAllInventariosAsync()
        {
            return await _context.Inventarios
                .Include(i => i.IdProductoNavigation)
                .Include(i => i.IdAlmacenNavigation)
                .ToListAsync();
        }
        public async Task<Inventario> GetInventarioAsync(int id)
        {
            return await _context.Inventarios
                .Include(i => i.IdProductoNavigation)
                .Include(i => i.IdAlmacenNavigation)
                .FirstOrDefaultAsync(i => i.IdInventario == id);
        }
        public async Task<Inventario?> ExistInventarioAsync(int id)
        {
            return await _context.Inventarios.FindAsync(id);
        }

        public async Task Update()
        {
            await _context.SaveChangesAsync();
        } 



    }
}
