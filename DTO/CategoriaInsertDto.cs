namespace SistemaInventario.DTO
{
    public class CategoriaInsertDto
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
