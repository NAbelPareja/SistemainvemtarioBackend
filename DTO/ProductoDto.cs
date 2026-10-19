namespace SistemaInventario.DTO
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; } = null!;
        public int IdMarca { get; set; }
        public string NombreMarca { get; set; } = null!;
    }
}
