namespace SistemaInventario.DTO
{
    public class DetalleCompraInsertDto
    {
       
        public int IdProducto { get; set; }
        public int IdAlmacen { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
