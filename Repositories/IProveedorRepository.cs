using SistemaInventario.Models;

namespace SistemaInventario.Repositories
{
    public interface IProveedorRepository
    {
        Task<IEnumerable<Proveedor>> GetAll();
        Task<Proveedor> GetProveedorId(int id);
        Task<bool?> NameExistAsync(string razonSocial);
        Task<Proveedor?> ProveedorExistAsync(int id);
        Task AddAsync(Proveedor proveedor);
        Task UpdateAsync(Proveedor proveedor);
        Task DeleteAsync(Proveedor proveedor);
    }
}
