namespace SistemaInventario.DTO
{
    public class VentaInsertDto
    {
        public int IdUsuarioRegistro { get; set; } // El cajero que procesa la venta
        public List<DetalleVentaInsertDto> Detalles { get; set; } = new();
    }
}
