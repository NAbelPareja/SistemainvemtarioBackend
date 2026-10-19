namespace SistemaInventario.DTO
{
    public class InventarioDto
    {
        public int Id { get; set; }       
        public int Stock { get; set; }
        public string StockMinimo { get; set; }
        public DateTime UltimoRegistro { get; set; }

        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string CodigoProducto { get; set; }


        public int IdAlmacen { get; set; }
        public string NombreAlmacen { get; set; }

    }
}
