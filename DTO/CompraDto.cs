namespace SistemaInventario.DTO
{
    public class CompraDto
    {
        public int IdCompra { get; set; }
        public string? ComprobanteReferencia { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = null!;  

        // Datos legibles de las relaciones
        public string RazonSocialProveedor { get; set; } = null!;
        public string RucProveedor { get; set; } = null!;
        public string NombreUsuarioRegistro { get; set; } = null!;

        // Lista de abarrotes incluidos en esta compra
        public List<DetalleCompraDto> Detalles { get; set; } = new();
    }
}
