namespace SistemaInventario.DTO
{
    public class AlmacenInsertDto
    {
        public string Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
