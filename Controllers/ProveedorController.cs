using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DTO;
using SistemaInventario.Models;
using SistemaInventario.Repositories;

namespace SistemaInventario.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorRepository _proveedorRepository;
        public ProveedorController(IProveedorRepository proveedorRepository)
        {
            _proveedorRepository = proveedorRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetAll()
        {
            var proveedores = await _proveedorRepository.GetAll();
            var proveedorDatos = proveedores.Select(p => new ProveedorDto
            {
                Id = p.IdProveedor,
                RazonSocial = p.RazonSocial,
                Ruc = p.Ruc,
                Correo = p.Correo,
                Telefono = p.Telefono,
                Direccion = p.Direccion,
                Estado = p.Estado
            }).ToList();
            return Ok(proveedorDatos);
        }

        [HttpGet("Id")]
        public async Task<ActionResult<Proveedor>> GetId(int id)
        {
            var proveedores = await _proveedorRepository.GetProveedorId(id);
            var proveedorDatos = new ProveedorDto
            {
                Id = proveedores.IdProveedor,
                RazonSocial = proveedores.RazonSocial,
                Ruc = proveedores.Ruc,
                Correo = proveedores.Correo,
                Telefono = proveedores.Telefono,
                Direccion = proveedores.Direccion,
                Estado = proveedores.Estado
            };
            return Ok(proveedorDatos);
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ProveedorInsertDto proveedor)
        {
            if (proveedor == null)
            {
                return BadRequest();
            }
            var nameExists = await _proveedorRepository.NameExistAsync(proveedor.RazonSocial);
            if (nameExists == true)
            {
                return NotFound("El nombre del proveedor ya existe.");
            }
            var proveedorEntity = new Proveedor
            {
                RazonSocial = proveedor.RazonSocial,
                Ruc = proveedor.Ruc,
                Correo = proveedor.Correo,
                Telefono = proveedor.Telefono,
                Direccion = proveedor.Direccion,
                Estado = proveedor.Estado
            };
            await _proveedorRepository.AddAsync(proveedorEntity);
            return CreatedAtAction(nameof(GetAll), new { id = proveedorEntity.IdProveedor }, proveedor);
        }

        [HttpPut]
        public async Task<ActionResult> Update(int id, [FromBody] ProveedorUpdateDto proveedor)
        {
            if (id != proveedor.Id)
            {
                return BadRequest("El id del proveedor no coincide.");
            }
            var proveedorExist = await _proveedorRepository.ProveedorExistAsync(id);
            if (proveedorExist == null)
            {
                return NotFound("El proveedor no existe.");
            }
            proveedorExist.RazonSocial = proveedor.RazonSocial;
            proveedorExist.Ruc = proveedor.Ruc;
            proveedorExist.Correo = proveedor.Correo;
            proveedorExist.Telefono = proveedor.Telefono;
            proveedorExist.Direccion = proveedor.Direccion;
            proveedorExist.Estado = proveedor.Estado;
            await _proveedorRepository.UpdateAsync(proveedorExist);
            return NoContent();
        }

        [HttpPut("Estado")]
        public async Task<ActionResult> Update(int id, [FromBody] bool estado)
        {
            if (id <=0)
            {
                return BadRequest("El id es valor no valido");
            }
            var proveedorExist = await _proveedorRepository.ProveedorExistAsync(id);
            if (proveedorExist == null)
            {
                return NotFound("El proveedor no existe.");
            }

            proveedorExist.Estado = estado;
            await _proveedorRepository.UpdateAsync(proveedorExist);
            return NotFound();

        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var proveedorExist = await _proveedorRepository.ProveedorExistAsync(id);
            if (proveedorExist == null)
            {
                return NotFound("El proveedor no existe.");
            }
            await _proveedorRepository.DeleteAsync(proveedorExist);
            return NoContent();
        }
    }
}
