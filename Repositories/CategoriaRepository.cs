using Microsoft.EntityFrameworkCore;
using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public class CategoriaRepository: ICategoriaRepository
    {
        private readonly SistemaInventarioContext _context;
        public CategoriaRepository(SistemaInventarioContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Categorium>> GetAll()
        {
            return await _context.Categoria.ToListAsync();
        }
        public async Task<Categorium> GetCategoriaId(int id)
        {
            return await _context.Categoria.FirstOrDefaultAsync(c => c.IdCategoria == id);
        }

        public async Task<bool?> NameExistAsync(string name)
        {
            return await _context.Categoria.AnyAsync(b => b.Nombre == name);
        }

        public async Task<Categorium?> CategoriaExistAsync(int id)
        {
            return await _context.Categoria.FindAsync(id);
        }

        public async Task AddAsync(Categorium categoria)
        {
            _context.Categoria.Add(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Categorium categoria)
        {
            _context.Categoria.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Categorium categoria)
        {
            categoria.Estado = false;
            _context.Categoria.Update(categoria);
            await _context.SaveChangesAsync();
        }

    }
}
