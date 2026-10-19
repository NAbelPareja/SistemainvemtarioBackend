namespace SistemaInventario.DTO
{
    public class UsuarioUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string? Contraseña { get; set; }
        public int? IdRol { get; set; }
    }
}
