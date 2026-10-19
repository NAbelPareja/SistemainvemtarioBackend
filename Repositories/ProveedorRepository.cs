using Microsoft.EntityFrameworkCore;
using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public class ProveedorRepository: IProveedorRepository
    {
        private readonly SistemaInventarioContext _context;
        public ProveedorRepository(SistemaInventarioContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Proveedor>> GetAll()
        {
            return await _context.Proveedors.ToListAsync();
        }
        public async Task<Proveedor> GetProveedorId(int id)
        {
            return await _context.Proveedors.FirstOrDefaultAsync(p => p.IdProveedor == id);
        }

        public async Task<bool?> NameExistAsync(string razonSocial)
        {
            return  await _context.Proveedors.AnyAsync(p => p.RazonSocial == razonSocial);
        }

        public async Task<Proveedor?> ProveedorExistAsync(int id)
        {
            return await _context.Proveedors.FindAsync(id);
        }

        public async Task AddAsync(Proveedor proveedor)
        {
            _context.Proveedors.Add(proveedor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Proveedor proveedor)
        {
            _context.Proveedors.Update(proveedor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Proveedor proveedor)
        {
            proveedor.Estado = false;
            _context.Proveedors.Update(proveedor);
            await _context.SaveChangesAsync();
        }

    }
}
