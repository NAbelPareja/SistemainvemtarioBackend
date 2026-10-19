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
    public class ProductoController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;
        public ProductoController(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }
        [HttpGet("Todos")]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAllProductos()
        {
            var productos = await _productoRepository.GetAllProductosAsync();

            var Productos = productos.Select(p => new ProductoDto
            {
                Id = p.IdProducto,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion != null ? p.Descripcion : "Sin Descripcion",
                PrecioCompra = p.PrecioCompra,
                PrecioVenta = p.PrecioVenta,
                Estado = p.Estado,
                FechaRegistro = p.FechaCreacion,
                IdCategoria = p.IdCategoria,
                NombreCategoria = p.IdCategoriaNavigation != null ? p.IdCategoriaNavigation.Nombre : "Sin categoria",
                IdMarca = p.IdMarca,
                NombreMarca = p.IdMarcaNavigation != null ? p.IdMarcaNavigation.Nombre : "Sin marca"
            }).ToList();

            return Ok(Productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var producto = await _productoRepository.GetProductoAsync(id);
            if (producto == null)
            {
                return NotFound("No existe el producto");
            }
            var productoDto = new ProductoDto
            {
                Id = producto.IdProducto,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion != null ? producto.Descripcion : "Sin Descripcion",
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                Estado = producto.Estado,
                FechaRegistro = producto.FechaCreacion,
                IdCategoria = producto.IdCategoria,
                NombreCategoria = producto.IdCategoriaNavigation != null ? producto.IdCategoriaNavigation.Nombre : "Sin categoria",
                IdMarca = producto.IdMarca,
                NombreMarca = producto.IdMarcaNavigation != null ? producto.IdMarcaNavigation.Nombre : "Sin marca"
            };
            return Ok(productoDto);
        }

        [Authorize(Roles = "Administrador,Encargado Almacen")]
        [HttpPost("Agregar")]
        public async Task<ActionResult> Add(ProductoInsertDto producto)
        {
            if (producto == null)
            {
                return BadRequest();
            }
            var existName = await _productoRepository.ExistName(producto.Nombre);
            if (existName)
            {
                return BadRequest("Ya existe un producto con ese nombre");
            }
            var existCategoria = await _productoRepository.ExistCategoriaAsync(producto.IdCategoria);
            if (existCategoria == null)
            {
                return BadRequest("No existe la categoria");
            }
            var ExistMarca = await _productoRepository.ExistMarcaAsync(producto.IdMarca);
            if (ExistMarca == null)
            {
                return BadRequest("No existe la marca");
            }

            var newProducto = new Producto
            {
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                Estado = producto.Estado,
                FechaCreacion = DateTime.Now,
                IdCategoria = producto.IdCategoria,
                IdMarca = producto.IdMarca
            };

            await _productoRepository.AddProductoAsync(newProducto);
            return Ok("Producto agregado correctamente");

        }

        [Authorize(Roles = "Administrador,Encargado Almacen")]
        [HttpPut("Actualizar/{id}")]
        public async Task<ActionResult> Update(int id, ProductoInsertDto producto)
        {
            if (id != producto.IdCategoria)
            {
                return BadRequest("El id del producto no coincide");
            }
            var existProducto = await _productoRepository.ExistProductoAsync(id);
            if (existProducto == null)
            {
                return NotFound("No existe el producto");
            }
            var existCategoria = await _productoRepository.ExistCategoriaAsync(producto.IdCategoria);
            if (existCategoria == null)
            {
                return BadRequest("No existe la categoria");
            }
            var ExistMarca = await _productoRepository.ExistMarcaAsync(producto.IdMarca);
            if (ExistMarca == null)
            {
                return BadRequest("No existe la marca");
            }
            existProducto.Codigo = producto.Codigo;
            existProducto.Nombre = producto.Nombre;
            existProducto.Descripcion = producto.Descripcion;
            existProducto.PrecioCompra = producto.PrecioCompra;
            existProducto.PrecioVenta = producto.PrecioVenta;
            existProducto.Estado = producto.Estado;
            existProducto.IdCategoria = producto.IdCategoria;
            existProducto.IdMarca = producto.IdMarca;
            await _productoRepository.UpdateProductoAsync(existProducto);
            return Ok("Producto actualizado correctamente");
        }

        [Authorize(Roles = "Administrador,Encargado Almacen")]
        [HttpPut("Estado")]
        public async Task<ActionResult> Update(int id, bool estado)
        {
            if (id <=0)
            {
                BadRequest("El valor ingresado no es valido");
            }
            var existProducto = await _productoRepository.ExistProductoAsync(id);
            if (existProducto == null)
            {
                return NotFound("El Producto no existe.");
            }

            existProducto.Estado = estado;

            await _productoRepository.UpdateProductoAsync(existProducto);
            return NoContent();
        }

        [Authorize(Roles = "Administrador,Encargado Almacen")]
        [HttpDelete("Eliminar/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existProducto = await _productoRepository.ExistProductoAsync(id);
            if (existProducto == null)
            {
                return NotFound("No existe el producto");
            }
            await _productoRepository.DeleteProductoAsync(existProducto);
            return Ok("Producto eliminado correctamente");
        }
    }
}
