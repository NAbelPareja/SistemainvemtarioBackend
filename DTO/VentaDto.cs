using SistemaInventario.Models;

namespace SistemaInventario.DTO
{
    public class VentaDto
    {
        public int IdVenta { get; set; }

        public int IdUsuarioRegistro { get; set; }

        public DateTime FechaVenta { get; set; }

        public decimal Total { get; set; }

        public string Estado { get; set; } = null!;

        public List<DetalleVentaDto> Detalles { get; set; } = new();
    }
}
