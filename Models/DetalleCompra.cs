using System;
using System.Collections.Generic;

namespace SistemaInventario.Models;

public partial class DetalleCompra
{
    public int IdDetalleCompra { get; set; }

    public int IdCompra { get; set; }

    public int IdProducto { get; set; }

    public int IdAlmacen { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioCompra { get; set; }

    public decimal? Subtotal { get; set; }

    public virtual Almacen IdAlmacenNavigation { get; set; } = null!;

    public virtual Compra IdCompraNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
