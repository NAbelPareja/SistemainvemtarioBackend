namespace SistemaInventario.DTO
{
    public class ProveedorInsertDto
    {
        public string RazonSocial { get; set; } = null!;
        public string Ruc { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public bool Estado { get; set; }
    }
}
