namespace SistemaInventario.DTO
{
    public class CompraInsertDto
    {
        public string? ComprobanteReferencia { get; set; } // Nro de Factura (ej: F001-001254)
        public int IdProveedor { get; set; }
        public int IdProducto { get; set; }

        // El bloque maestro-detalle anidado:
        public List<DetalleCompraInsertDto> Detalles { get; set; } = new();
    }
}
