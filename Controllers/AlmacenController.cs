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
    public class AlmacenController : ControllerBase
    {
        private readonly IAlmacenRepository _almacenRepository;
        public AlmacenController(IAlmacenRepository almacenRepository)
        {
            _almacenRepository = almacenRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlmacenDto>>> GetAll()
        {
            var almacenes = await _almacenRepository.GetAll();
            var almacenDatos = almacenes.Select(a => new AlmacenDto
            {
                Id = a.IdAlmacen,
                Nombre = a.Nombre,
                Direccion = a.Direccion,
                Descripcion = a.Descripcion,
                Estado = a.Estado
            }).ToList();
            return Ok(almacenDatos);
        }
          
        [HttpGet("{Id}")]
        public async Task<ActionResult<IEnumerable<AlmacenDto>>> GetId(int id)
        {
            var almacenes = await _almacenRepository.GetAlmacenId(id);

            if (almacenes == null)
            {
                return BadRequest("El almacen no existe o el id es invalido");
            }
            var almacenDatos =  new AlmacenDto
            {
                Id = almacenes.IdAlmacen,
                Nombre = almacenes.Nombre,
                Direccion = almacenes.Direccion,
                Descripcion = almacenes.Descripcion,
                Estado = almacenes.Estado
            };
            return Ok(almacenDatos);
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] AlmacenInsertDto almacen)
        {
            if (almacen == null)
            {
                return BadRequest();
            }
            var nameExists = await _almacenRepository.NameExistAsync(almacen.Nombre);
            if (nameExists)
            {
                return NotFound("El nombre del almacén ya existe.");
            }
            var almacenEntity = new Almacen
            {
                Nombre = almacen.Nombre,
                Direccion = almacen.Direccion,
                Descripcion = almacen.Descripcion,
                Estado = true
            };
            await _almacenRepository.AddAsync(almacenEntity);
            return CreatedAtAction(nameof(GetAll), new { id = almacenEntity.IdAlmacen }, almacen);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] AlmacenUpdateDto almacen)
        {
            if (id != almacen.Id)
            {
                return BadRequest("El id del almacén no coincide.");
            }
            var almacenExist = await _almacenRepository.AlmacenExistAsync(id);
            if (almacenExist == null)
            {
                return NotFound("El almacén no existe.");
            }
            almacenExist.Nombre = almacen.Nombre;
            almacenExist.Direccion = almacen.Direccion;
            almacenExist.Descripcion = almacen.Descripcion;
            await _almacenRepository.UpdateAsync(almacenExist);
            return NoContent();
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var almacenExist = await _almacenRepository.AlmacenExistAsync(id);
            if (almacenExist == null)
            {
                return NotFound("El almacén no existe.");
            }
            await _almacenRepository.DeleteAsync(almacenExist);
            return NoContent();
        }
    }
}
