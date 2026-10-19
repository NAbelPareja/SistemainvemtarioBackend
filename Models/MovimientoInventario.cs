using System;
using System.Collections.Generic;

namespace SistemaInventario.Models;

public partial class MovimientoInventario
{
    public int IdMovimiento { get; set; }

    public int IdProducto { get; set; }

    public int IdAlmacen { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public int Cantidad { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public int IdUsuario { get; set; }

    public string? Referencia { get; set; }

    public virtual Almacen IdAlmacenNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
