namespace SistemaInventario.DTO
{
    public class InventarioUpdateDto
    {
        public int Id { get; set; }
        public int Stock { get; set; }
        public string StockMinimo { get; set; }
        public DateTime UltimoRegistro { get; set; }

        public int IdProducto { get; set; }
        public int IdAlmacen { get; set; }

    }
}
