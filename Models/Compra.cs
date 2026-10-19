using System;
using System.Collections.Generic;

namespace SistemaInventario.Models;

public partial class Compra
{
    public int IdCompra { get; set; }

    public string? ComprobanteReferencia { get; set; }

    public int IdProveedor { get; set; }

    public int IdUsuarioRegistro { get; set; }

    public DateTime FechaCompra { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioRegistroNavigation { get; set; } = null!;
}
