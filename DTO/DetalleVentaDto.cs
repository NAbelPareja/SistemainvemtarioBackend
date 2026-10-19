namespace SistemaInventario.DTO
{
    public class DetalleVentaDto
    {
        public int IdDetalleVenta { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = null!;
        public string CodigoProducto { get; set; } = null!;
        public int IdAlmacen { get; set; }
        public string NombreAlmacen { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal Subtotal { get; set; } // Campo calculado de SQl
    }
}
