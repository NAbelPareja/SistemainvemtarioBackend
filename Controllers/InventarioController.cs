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
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioRepository _inventarioRepository;
        public InventarioController(IInventarioRepository inventarioRepository)
        {
            _inventarioRepository = inventarioRepository;
        }
        [HttpGet]
        public async Task<ActionResult<InventarioDto>> GetAllInventarios()
        {
            var inventarios = await _inventarioRepository.GetAllInventariosAsync();
            var ListaInventario = inventarios.Select(p => new InventarioDto
            {
                Id = p.IdInventario,
                IdProducto = p.IdProducto,
                NombreProducto = p.IdProductoNavigation != null ? p.IdProductoNavigation.Nombre : "Sin producto",
                IdAlmacen = p.IdAlmacen,
                NombreAlmacen = p.IdAlmacenNavigation != null ? p.IdAlmacenNavigation.Nombre : "Sin almacen",
                Stock = p.Stock,
                UltimoRegistro = p.UltimaActualizacion,
            }).ToList();

            return Ok(ListaInventario);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventario(int id)
        {
            var inventario = await _inventarioRepository.GetInventarioAsync(id);
            if (inventario == null)
            {
                return NotFound("El inventario no existe");
            }
            var ListaInventario = new InventarioDto
            {
                Id = inventario.IdInventario,
                IdProducto = inventario.IdProducto,
                NombreProducto = inventario.IdProductoNavigation != null ? inventario.IdProductoNavigation.Nombre : "Sin producto",
                IdAlmacen = inventario.IdAlmacen,
                NombreAlmacen = inventario.IdAlmacenNavigation != null ? inventario.IdAlmacenNavigation.Nombre : "Sin almacen",
                Stock = inventario.Stock,
                UltimoRegistro = inventario.UltimaActualizacion,
            };

            return Ok(ListaInventario);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateInventario(int id,[FromBody] InventarioUpdateDto inventarioDto)
        {
            if (id != inventarioDto.Id)
            {
                return BadRequest("Los ids no coninciden");
            }
            var InventarioExistente = await _inventarioRepository.ExistInventarioAsync(id);

            InventarioExistente.Stock = inventarioDto.Stock;
            InventarioExistente.UltimaActualizacion = DateTime.Now;
            await _inventarioRepository.Update();
            return Ok("Inventario actualizado correctamente");
        }
    }
}
