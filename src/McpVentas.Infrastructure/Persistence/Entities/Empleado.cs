using System;

namespace McpVentas.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad que representa la tabla <c>empleado</c> (catalogo de vendedores).
/// Se usa para enriquecer <c>venta.vendedor</c> con el nombre completo.
/// </summary>
public partial class Empleado
{
    /// <summary>Identificador unico del empleado. Clave primaria.</summary>
    public Guid id_empleado { get; set; }

    /// <summary>Nombre(s) del empleado.</summary>
    public string nombre { get; set; } = null!;

    /// <summary>Apellido paterno.</summary>
    public string a_paterno { get; set; } = null!;

    /// <summary>Apellido materno.</summary>
    public string a_materno { get; set; } = null!;

    /// <summary>Nombre de usuario (se cruza con <c>venta.vendedor</c>).</summary>
    public string user_name { get; set; } = null!;

    /// <summary>Fecha de alta del empleado.</summary>
    public DateTime fecha_registro { get; set; }
}
