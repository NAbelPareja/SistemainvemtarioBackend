
namespace SistemaInventario.DTO
{
    public class UsuarioInsertDto
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string? Contraseña { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? IdRol { get; set; }

    }
}
