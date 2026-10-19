using Microsoft.EntityFrameworkCore;
using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public class AlmacenRepository : IAlmacenRepository
    {
        private readonly SistemaInventarioContext _context;
        public AlmacenRepository(SistemaInventarioContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Almacen>> GetAll()
        {
            return await _context.Almacens.ToListAsync();
        }
        public async Task<Almacen> GetAlmacenId(int id)
        {
            return await _context.Almacens.FirstOrDefaultAsync(a => a.IdAlmacen == id);
        }
        public async Task<bool> NameExistAsync(string name)
        {
            return await _context.Almacens.AnyAsync(a => a.Nombre == name);
        }

        public async Task<Almacen?> AlmacenExistAsync(int id)
        {
            return await _context.Almacens.FindAsync(id);
        }

        public async Task AddAsync(Almacen almacen)
        {
            _context.Almacens.Add(almacen);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Almacen almacen)
        {
            _context.Almacens.Update(almacen);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Almacen almacen)
        {
            almacen.Estado = false;
            _context.Almacens.Update(almacen);
            await _context.SaveChangesAsync();
        }

    }
}
