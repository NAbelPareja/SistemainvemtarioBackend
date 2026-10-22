using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DTO;
using SistemaInventario.Models;
using SistemaInventario.Repositories;
using System.Security.Claims;

namespace SistemaInventario.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador,Vendedor")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly IVentaRepository _ventaRepository;
        public VentaController(IVentaRepository ventaRepository)
        {
            _ventaRepository = ventaRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDto>>> Get()
        {
            // 1. Traemos las compras con todas sus relaciones profundas del repositorio
            var ventas = await _ventaRepository.GetAll();
            // Validación de seguridad perimetral: Evitar que manden una grilla vacía
            var comprasDto = ventas.Select(c => new VentaDto
            {
                IdVenta = c.IdVenta,
                FechaVenta = c.FechaVenta,
                Total = c.Total,
                Estado = c.Estado,

                IdUsuarioRegistro = c.IdUsuarioRegistro,

                // MAPEO DEL DETALLE: Recorremos los productos de cada compra

                Detalles = c.DetalleVenta.Select(d => new DetalleVentaDto
                {
                    IdDetalleVenta = d.IdDetalleVenta,
                    IdProducto = d.IdProducto,
                    Cantidad = d.Cantidad,
                    PrecioCompra = d.PrecioUnitario,
                    Subtotal = (int)d.Subtotal, // Campo calculado de la BD

                    // Nombres de los abarrotes y almacenes gracias al ThenInclude
                    NombreProducto = d.IdProductoNavigation != null ? d.IdProductoNavigation.Nombre : "Producto Eliminado",
                    CodigoProducto = d.IdProductoNavigation != null ? d.IdProductoNavigation.Codigo : "-",
                    IdAlmacen = d.IdAlmacen,
                    NombreAlmacen = d.IdAlmacenNavigation != null ? d.IdAlmacenNavigation.Nombre : "Almacén No Asignado"
                }).ToList()
            }).ToList();
            return Ok(comprasDto);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<VentaDto>> GetID(int id)
        {
            var venta = await _ventaRepository.GetId(id);

            if (venta == null)
            {
                return NotFound($"No se encontro el producto con el id: {id}.");
            }

            // Mapeamos el objeto individual sin usar .Select() en la cabecera
            var compraDto = new VentaDto
            {
                IdVenta= venta.IdVenta,
                FechaVenta = venta.FechaVenta,
                Total = venta.Total,
                Estado = venta.Estado,
                    
                
                // Los detalles internos sí son una lista, así que aquí sí usamos .Select()
                Detalles = venta.DetalleVenta.Select(d => new DetalleVentaDto
                {
                    IdDetalleVenta = d.IdDetalleVenta,
                    IdProducto = d.IdProducto,
                    Cantidad = d.Cantidad,
                    PrecioCompra = d.PrecioUnitario,
                    Subtotal = (int)d.Subtotal,

                    NombreProducto = d.IdProductoNavigation != null ? d.IdProductoNavigation.Nombre : "Producto Eliminado",
                    CodigoProducto = d.IdProductoNavigation != null ? d.IdProductoNavigation.Codigo : "-",
                    IdAlmacen = d.IdAlmacen,
                    NombreAlmacen = d.IdAlmacenNavigation != null ? d.IdAlmacenNavigation.Nombre : "Almacén No Asignado"
                }).ToList()
            };

            return Ok(compraDto);
        }

        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarVenta([FromBody] VentaInsertDto dto)
        {
            // 1. Extraes el ID del usuario logueado desde los Claims del Token JWT
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // 2. Compruebas el estado actual en la Base de Datos (en tiempo real)
            var usuarioActivo = await  _ventaRepository.UsuarioActivoAsync(usuarioId);

            if (!usuarioActivo)
            {
                return Unauthorized("Tu usuario ya no se encuentra activo en el sistema.");
            }

            // 1. Validar que el JSON no venga vacío
            if (dto == null || dto.Detalles.Count == 0)
            {
                return BadRequest("La compra debe contener al menos un producto en el detalle.");
            }

            // 🔒 2. ¡EL TRUCO DE SEGURIDAD! Extraes el ID directamente del Token JWT
            // Revisa cómo llamaste al Claim en tu TokenService (NameIdentifier o Name)
            var claimUsuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claimUsuarioId))
            {
                return Unauthorized("No se pudo identificar al usuario logueado a través del token.");
            }

            int usuarioLogueadoId = int.Parse(claimUsuarioId);

            // 3. Mapear de forma limpia el DTO Maestro-Detalle hacia tus Entidades Reales de la BD
            var nuevaVenta = new Ventum
            {
                IdUsuarioRegistro = usuarioLogueadoId, // Vinculamos el usuario auditor
                FechaVenta = DateTime.Now,
                Estado = "Procesado",
                // Calculamos el Total sumando los subtotales de forma matemática en memoria
                Total = dto.Total,
            };



            // 4. Mapeamos la lista interna de detalles recorriendo el DTO de React
            foreach (var item in dto.Detalles)
            {
                // 🔒 Aduana del Almacén de destino
                if (!await _ventaRepository.AlmacenActivoAsync(item.IdAlmacen))
                {
                    return BadRequest($"El almacén con ID {item.IdAlmacen} no existe o está inactivo.");
                }

                // 🔒 Aduana del Producto
                if (!await _ventaRepository.ProductoActivoAsync(item.IdProducto))
                {
                    return BadRequest($"El producto con ID {item.IdProducto} no existe o está descontinuado.");
                }

                    var detalleEntidad = new DetalleVentum
                {

                    IdProducto = item.IdProducto,
                    IdAlmacen = item.IdAlmacen, // Destino físico del stock
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
                };
                // Agregamos el hijo a la propiedad de navegación de la cabecera
                nuevaVenta.DetalleVenta.Add(detalleEntidad);
            }

            // 5. Mandamos la entidad armada al superpoder transaccional de tu repositorio
            try
            {
                await _ventaRepository.Insert(nuevaVenta);
                return Ok(new { mensaje = "Compra registrada con éxito y stock actualizado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al procesar la logística: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> AnularVenta(int id)
        {
            var venta = await _ventaRepository.VentaExistasync(id);
            if (venta == null) return NotFound("La venta no existe.");

            if (venta.Estado == "Anulado") return BadRequest("Esta venta ya se encuentra anulada.");

            // 🔄 Tu borrado lógico transaccional:
            venta.Estado = "Anulado";

            // 🛒 REVERSIÓN DE STOCK (Kardex): 
            // Recorres los detalles para devolver la mercadería al almacén
            foreach (var detalle in venta.DetalleVenta)
            {
                var inventario = await _ventaRepository.ObetenerInventario(detalle);
                if (inventario != null)
                {
                    inventario.Stock += detalle.Cantidad; // Devuelves el stock // Devuelves el stock que se había llevado
                }
            }

            await _ventaRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
