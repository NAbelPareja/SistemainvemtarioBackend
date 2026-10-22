namespace SistemaInventario.DTO
{
    public class VentaInsertDto
    {
        public decimal Total { get; set; } 
        public List<DetalleVentaInsertDto> Detalles { get; set; } = new();
    }
}
