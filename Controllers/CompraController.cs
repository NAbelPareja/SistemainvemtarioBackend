using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DTO;
using SistemaInventario.Models;
using SistemaInventario.Repositories;

namespace SistemaInventario.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador,Encargado Almacen")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        private readonly IComprasRepository _comprasRepository;
        public CompraController(IComprasRepository comprasRepository)
        {
            _comprasRepository = comprasRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDto>>> Get()
        {
            // 1. Traemos las compras con todas sus relaciones profundas del repositorio
            var compras = await _comprasRepository.GetAll();

            // 2. Mapeamos la lista de entidades hacia la lista de DTOs
            var comprasDto = compras.Select(c => new CompraDto
            {
                IdCompra = c.IdCompra,
                ComprobanteReferencia = c.ComprobanteReferencia,
                FechaCompra = c.FechaCompra,
                Total = c.Total,
                Estado = c.Estado,

                // Nombres legibles de las tablas maestras
                RazonSocialProveedor = c.IdProveedorNavigation != null ? c.IdProveedorNavigation.RazonSocial : "Sin Proveedor",
                RucProveedor = c.IdProveedorNavigation != null ? c.IdProveedorNavigation.Ruc : "-",
                NombreUsuarioRegistro = c.IdUsuarioRegistroNavigation != null ? $"{c.IdUsuarioRegistroNavigation.Nombre} {c.IdUsuarioRegistroNavigation.Apellido}" : "Sistema",

                // MAPEO DEL DETALLE: Recorremos los productos de cada compra
                Detalles = c.DetalleCompras.Select(d => new DetalleCompraDto
                {
                    IdDetalleCompra = d.IdDetalleCompra,
                    IdProducto = d.IdProducto,
                    Cantidad = d.Cantidad,
                    PrecioCompra = d.PrecioCompra,
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
        public async Task<ActionResult<Compra>> getId(int id)
        {
            var compra = await _comprasRepository.GetId(id);

            if (compra == null)
            {
                return NotFound($"No se encontro el producto con el id: {id}.");
            }

            // Mapeamos el objeto individual sin usar .Select() en la cabecera
            var compraDto = new CompraDto
            {
                IdCompra = compra.IdCompra,
                ComprobanteReferencia = compra.ComprobanteReferencia,
                FechaCompra = compra.FechaCompra,
                Total = compra.Total,
                Estado = compra.Estado,

                RazonSocialProveedor = compra.IdProveedorNavigation != null ? compra.IdProveedorNavigation.RazonSocial : "Sin Proveedor",
                RucProveedor = compra.IdProveedorNavigation != null ? compra.IdProveedorNavigation.Ruc : "-",
                NombreUsuarioRegistro = compra.IdUsuarioRegistroNavigation != null ? $"{compra.IdUsuarioRegistroNavigation.Nombre} {compra.IdUsuarioRegistroNavigation.Apellido}" : "Sistema",

                // Los detalles internos sí son una lista, así que aquí sí usamos .Select()
                Detalles = compra.DetalleCompras.Select(d => new DetalleCompraDto
                {
                    IdDetalleCompra = d.IdDetalleCompra,
                    IdProducto = d.IdProducto,
                    Cantidad = d.Cantidad,
                    PrecioCompra = d.PrecioCompra,
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
        public async Task<IActionResult> RegistrarCompra([FromBody] CompraInsertDto dto)
        {
            // 1. Validar que el JSON no venga vacío
            if (dto == null || !dto.Detalles.Any())
            {
                return BadRequest("La compra debe contener al menos un producto en el detalle.");
            }

            // 2. Tu validación estrella: Evitar duplicados usando tu método ComprovanteExiste
            if (!string.IsNullOrEmpty(dto.ComprobanteReferencia))
            {
                var yaExiste = await _comprasRepository.ComprovanteExiste(dto.ComprobanteReferencia);
                if (yaExiste)
                {
                    return BadRequest($"La factura o comprobante '{dto.ComprobanteReferencia}' ya fue registrado anteriormente.");
                }
            }

            // 3. Mapear de forma limpia el DTO Maestro-Detalle hacia tus Entidades Reales de la BD
            var nuevaCompra = new Compra
            {
                ComprobanteReferencia = dto.ComprobanteReferencia,
                IdProveedor = dto.IdProveedor,
                IdUsuarioRegistro = dto.IdUsuarioRegistro, // Vinculamos el usuario auditor
                FechaCompra = DateTime.Now,
                Estado = "Procesado",
                // Calculamos el Total sumando los subtotales de forma matemática en memoria
                Total = dto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario)
            };

            // 4. Mapeamos la lista interna de detalles recorriendo el DTO de React
            foreach (var item in dto.Detalles)
            {
                var detalleEntidad = new DetalleCompra
                {
                    IdProducto = item.IdProducto,
                    IdAlmacen = item.IdAlmacen, // Destino físico del stock
                    Cantidad = item.Cantidad,
                    PrecioCompra = item.PrecioUnitario
                };

                // Agregamos el hijo a la propiedad de navegación de la cabecera
                nuevaCompra.DetalleCompras.Add(detalleEntidad);
            }

            // 5. Mandamos la entidad armada al superpoder transaccional de tu repositorio
            try
            {
                await _comprasRepository.Insert(nuevaCompra);
                return Ok(new { mensaje = "Compra registrada con éxito y stock actualizado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al procesar la logística: {ex.Message}");
            }
        }

    }
}
