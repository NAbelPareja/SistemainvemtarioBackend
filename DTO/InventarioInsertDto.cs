namespace SistemaInventario.DTO
{
    public class InventarioInsertDto
    {
        public int Stock { get; set; }
        public string StockMinimo { get; set; }
        public DateTime UltimoRegistro { get; set; }

        public int IdProducto { get; set; }

        public int IdAlmacen { get; set; }

    }
}
