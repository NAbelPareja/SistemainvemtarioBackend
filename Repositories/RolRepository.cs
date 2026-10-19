using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly SistemaInventarioContext _context;
        public RolRepository(SistemaInventarioContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _context.Rols.ToListAsync();
        }

        public async Task<bool?> NameExistasync(string name)
        {
            return await _context.Rols.AnyAsync(b => b.Nombre == name);
        }

        public async Task<Rol?> RolexistAsync(int id)
        {
            return await _context.Rols.FindAsync(id);
        }

        public async Task AddAsync(Rol rol)
        {
            _context.Rols.Add(rol);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rol rol)
        {
            _context.Rols.Update(rol);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Rol rol)
        {
            _context.Rols.Remove(rol);
            await _context.SaveChangesAsync();
        }




    }
}
