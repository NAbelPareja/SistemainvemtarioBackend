namespace SistemaInventario.DTO
{
    public class DetalleVentaInsertDto
    {
        public int IdProducto { get; set; }
        public int IdAlmacen { get; set; }    // De qué almacén físico sale el abarrote
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; } // El precio de venta congelado
    }
}
