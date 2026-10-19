using System;
using System.Collections.Generic;

namespace SistemaInventario.Models;

public partial class Inventario
{
    public int IdInventario { get; set; }

    public int IdProducto { get; set; }

    public int IdAlmacen { get; set; }

    public int Stock { get; set; }

    public int StockMinimo { get; set; }

    public DateTime UltimaActualizacion { get; set; }

    public virtual Almacen IdAlmacenNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
