using Microsoft.EntityFrameworkCore;
using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public class MarcaRepository : IMarcaRepository
    {   
        private readonly SistemaInventarioContext _context;
        public MarcaRepository(SistemaInventarioContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Marca>> GetAllAsync()
        {
            return await _context.Marcas.ToListAsync();
        }
        public async Task<Marca> GetMarcaId(int id)
        {
            return await _context.Marcas.FirstOrDefaultAsync(m => m.IdMarca == id);
        }
        public async Task<bool?> NameExistAsync(string name)
        {
            return await _context.Marcas.AnyAsync(b => b.Nombre == name);
        }

        public async Task<Marca?> MarcaExistAsync(int id)
        {
            return await _context.Marcas.FindAsync(id);
        }
        public async Task AddAsync(Marca marca)
        {
            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Marca marca)
        {
            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();
        }
    }
}
