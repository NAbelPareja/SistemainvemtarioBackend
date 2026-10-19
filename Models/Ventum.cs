using System;
using System.Collections.Generic;

namespace SistemaInventario.Models;

public partial class Ventum
{
    public int IdVenta { get; set; }
    public int IdUsuarioRegistro { get; set; }

    public DateTime FechaVenta { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<DetalleVentum> DetalleVenta { get; set; } = new List<DetalleVentum>();

    public virtual Usuario IdUsuarioRegistroNavigation { get; set; } = null!;


}
