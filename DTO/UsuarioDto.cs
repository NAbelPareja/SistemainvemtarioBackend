namespace SistemaInventario.DTO
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string? Contraseña { get; set; }
        public bool Estado { get; set; }
        public string FechaRegistro { get; set; } = null!;
        public string RolNombre { get; set; } = null!;
    }
}
