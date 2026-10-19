using System;
using System.Collections.Generic;

namespace SistemaInventario.Models;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string RazonSocial { get; set; } = null!;

    public string Ruc { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
