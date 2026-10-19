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
    public class RolController : ControllerBase
    {
        private readonly IRolRepository _rolRepository;
        public RolController(IRolRepository rolRepository)
        {
            _rolRepository = rolRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetAll()
        {
            var Rols = await _rolRepository.GetAllAsync();
            var rol = Rols.Select(r => new RolDto
            {
                Id = r.IdRol,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion
            });
            return Ok(rol);
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] RolInsertDto rolDto)
        {
            if (rolDto == null)
            {
                return BadRequest();
            }
            var nameExists = await _rolRepository.NameExistasync(rolDto.Nombre);
            if (nameExists == true)
            {
                return NotFound("El nombre del rol ya existe.");
            }

            var rol = new Rol
            {
                Nombre = rolDto.Nombre,
                Descripcion = rolDto.Descripcion
            };

            _rolRepository.AddAsync(rol);
            return CreatedAtAction(nameof(GetAll), new { id = rol.IdRol }, rol);
        }

        [HttpPut]
        public async Task<ActionResult> Update(int id, [FromBody] RolUpdateDto roldto)
        {
            if (id != roldto.Id)
            {
                return BadRequest("El id del rol no coincide.");
            }

            var existingRol = await _rolRepository.RolexistAsync(roldto.Id);

            if (existingRol == null)
            {
                return NotFound("El rol no existe.");
            }

            existingRol.Nombre = roldto.Nombre;
            existingRol.Descripcion = roldto.Descripcion;

            await _rolRepository.UpdateAsync(existingRol);
            return NoContent();

        }
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var rolExist = await _rolRepository.RolexistAsync(id);
            if (rolExist == null)
            {
                return NotFound("El rol no existe.");
            }
            await _rolRepository.DeleteAsync(rolExist);
            return NoContent();
        }


    }
}
