using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DTO;
using SistemaInventario.Models;
using SistemaInventario.Repositories;

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
        [HttpGet("id")]
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
        public async Task<IActionResult> RegistrarCompra([FromBody] VentaInsertDto dto)
        {
            // 1. Validar que el JSON no venga vacío
            if (dto == null || !dto.Detalles.Any())
            {
                return BadRequest("La compra debe contener al menos un producto en el detalle.");
            }


            // 3. Mapear de forma limpia el DTO Maestro-Detalle hacia tus Entidades Reales de la BD
            var nuevaVenta = new Ventum
            {
                IdUsuarioRegistro = dto.IdUsuarioRegistro, // Vinculamos el usuario auditor
                FechaVenta = DateTime.Now,
                Estado = "Procesado",
                // Calculamos el Total sumando los subtotales de forma matemática en memoria
                Total = dto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario)
            };

            // 4. Mapeamos la lista interna de detalles recorriendo el DTO de React
            foreach (var item in dto.Detalles)
            {
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
    }
}
