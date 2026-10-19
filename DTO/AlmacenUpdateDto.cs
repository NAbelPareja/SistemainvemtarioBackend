namespace SistemaInventario.DTO
{
    public class AlmacenUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
