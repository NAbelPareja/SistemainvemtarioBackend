namespace SistemaInventario.DTO
{
    public class CompraInsertDto
    {
        public string? ComprobanteReferencia { get; set; } // Nro de Factura (ej: F001-001254)
        public int IdProveedor { get; set; }
        public int IdUsuarioRegistro { get; set; } // El empleado que digita la compra

        // El bloque maestro-detalle anidado:
        public List<DetalleCompraInsertDto> Detalles { get; set; } = new();
    }
}
