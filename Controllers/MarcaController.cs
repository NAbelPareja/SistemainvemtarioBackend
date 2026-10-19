using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DTO;
using SistemaInventario.Models;
using SistemaInventario.Repositories;

namespace SistemaInventario.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MarcaController : ControllerBase
    {
        private readonly IMarcaRepository _marcaRepository;
        public MarcaController(IMarcaRepository marcaRepository)
        {
            _marcaRepository = marcaRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marca>>> GetAll()
        {
            var marcas = await _marcaRepository.GetAllAsync();

            var marcaDatos = marcas.Select(m => new MarcaDto
            {
                IdMarca = m.IdMarca,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion
            }).ToList();

            return Ok(marcaDatos);
        }

        [HttpGet("Id")]
        public async Task<ActionResult<Marca>> GetId(int id)
        {
            var marcas = await _marcaRepository.GetMarcaId(id);

            var marcaDatos =  new MarcaDto
            {
                IdMarca = marcas.IdMarca,
                Nombre = marcas.Nombre,
                Descripcion = marcas.Descripcion
            };

            return Ok(marcaDatos);
        }

        [Authorize(Roles = "Administrador,Encargado Almacen")]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] MarcaInsertDto marcaDto)
        {
            if (marcaDto == null)
            {
                return BadRequest();
            }
            var nameExists = await _marcaRepository.NameExistAsync(marcaDto.Nombre);
            if (nameExists == true)
            {
                return NotFound("El nombre de la marca ya existe.");
            }
            var marca = new Marca
            {
                Nombre = marcaDto.Nombre,
                Descripcion = marcaDto.Descripcion
            };
            await _marcaRepository.AddAsync(marca);
            return CreatedAtAction(nameof(GetAll), new { id = marca.IdMarca }, marca);
        }

        [Authorize(Roles = "Administrador,Encargado Almacen")]
        [HttpPut]
        public async Task<ActionResult> Update(int id, [FromBody] MarcaUpdateDto marcaDto)
        {
            if (id != marcaDto.IdMarca)
            {
                return BadRequest("El id de la marca no coincide.");
            }
            var marcaExist = await _marcaRepository.MarcaExistAsync(id);
            if (marcaExist == null)
            {
                return NotFound("La marca no existe.");
            }
            marcaExist.Nombre = marcaDto.Nombre;
            marcaExist.Descripcion = marcaDto.Descripcion;
            await _marcaRepository.UpdateAsync();
            return NoContent();
        }

        [Authorize(Roles = "Administrador,Encargado Almacen")]
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var marcaExist = await _marcaRepository.MarcaExistAsync(id);
            if (marcaExist == null)
            {
                return NotFound("La marca no existe.");
            }
            await _marcaRepository.DeleteAsync(marcaExist);
            return NoContent();
        }
    }
}
