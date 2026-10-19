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
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaRepository _categoriaRepository;
        public CategoriaController(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        [HttpGet]
        public  async Task<ActionResult<IEnumerable<Categorium>>> GetAll()
        {
            var categorias = await _categoriaRepository.GetAll();

            var categoriaDatos = categorias.Select(c => new CategoriaDto
            {
                Id = c.IdCategoria,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion
            }).ToList();
            return Ok(categoriaDatos);
        }

        [HttpGet("Id")]
        public async Task<ActionResult<Categorium>> GetId(int id)
        {
            var categorias = await _categoriaRepository.GetCategoriaId(id);

            var categoriaDatos = new CategoriaDto
            {
                Id = categorias.IdCategoria,
                Nombre = categorias.Nombre,
                Descripcion = categorias.Descripcion
            };
            return Ok(categoriaDatos);
        }

        [Authorize(Roles = "Administrador,Encargado Almacen")]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] CategoriaInsertDto categoria)
        {
            if (categoria == null)
            {
                return BadRequest();
            }
            var nameExists = await _categoriaRepository.NameExistAsync(categoria.Nombre);
            if (nameExists == true)
            {
                return NotFound("El nombre de la categoría ya existe.");
            }

            var categoriaEntity = new Categorium
            {
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Estado = categoria.Estado
            };

            await _categoriaRepository.AddAsync(categoriaEntity);
            return CreatedAtAction(nameof(GetAll), new { id = categoriaEntity.IdCategoria}, categoria);
        }

        [Authorize(Roles = "Administrador,Encargado Almacen")]
        [HttpPut]
        public async Task<ActionResult> Update(int id, [FromBody] CategoriaUpdateDto categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest("El id de la categoría no coincide.");
            }

            var categoriaExist = await _categoriaRepository.CategoriaExistAsync(id);
            if (categoriaExist == null)
            {
                return NotFound("La categoría no existe.");
            }
            categoriaExist.Nombre = categoria.Nombre;
            categoriaExist.Descripcion = categoria.Descripcion;
            categoriaExist.Estado = categoria.Estado;
            await _categoriaRepository.UpdateAsync(categoriaExist);
            return NoContent();
        }

        [Authorize(Roles = "Administrador,Encargado Almacen")]
        [HttpPut("Estado")]
        public async Task<ActionResult> Update(int id, [FromBody] bool estado)
        {
            if (id<= 0 )
            {
                BadRequest("Valor ingresado no valido");
            }
            var categoriaExist = await _categoriaRepository.CategoriaExistAsync(id);
            if (categoriaExist == null)
            {
                return NotFound("La categoría no existe.");
            }
            categoriaExist.Estado = estado;
            await _categoriaRepository.UpdateAsync(categoriaExist);
            return NoContent();
        }

        [Authorize(Roles = "Administrador,Encargado Almacen")]
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var categoriaExist = await _categoriaRepository.CategoriaExistAsync(id);
            if (categoriaExist == null)
            {
                return NotFound("La categoría no existe.");
            }
            await _categoriaRepository.DeleteAsync(categoriaExist);
            return Ok("valor inactivo");
        }
    }
}
