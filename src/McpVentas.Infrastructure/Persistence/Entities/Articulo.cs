using System;
using System.Collections.Generic;

namespace McpVentas.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad que representa la tabla <c>articulo</c> (catalogo de productos).
/// La clave primaria es <c>cod_barras</c>.
/// </summary>
public partial class Articulo
{
    /// <summary>Codigo de barras del articulo. Clave primaria.</summary>
    public string cod_barras { get; set; } = null!;

    /// <summary>Codigo asociado (opcional).</summary>
    public string? cod_asociado { get; set; }

    /// <summary>Identificador de la clasificacion/departamento.</summary>
    public long id_clasificacion { get; set; }

    /// <summary>Codigo interno del articulo (opcional).</summary>
    public string? cod_interno { get; set; }

    /// <summary>Descripcion comercial del articulo.</summary>
    public string descripcion { get; set; } = null!;

    /// <summary>Descripcion corta usada en tickets.</summary>
    public string descripcion_corta { get; set; } = null!;

    /// <summary>Cantidad por unidad de medida.</summary>
    public decimal cantidad_um { get; set; }

    /// <summary>Identificador de la unidad de medida.</summary>
    public Guid id_unidad { get; set; }

    /// <summary>Identificador del proveedor.</summary>
    public Guid id_proveedor { get; set; }

    /// <summary>Precio de compra del articulo.</summary>
    public decimal precio_compra { get; set; }

    /// <summary>Utilidad asociada al articulo.</summary>
    public decimal utilidad { get; set; }

    /// <summary>Precio de venta actual del articulo.</summary>
    public decimal precio_venta { get; set; }

    /// <summary>Tipo/clasificacion del articulo (p. ej. "principal").</summary>
    public string tipo_articulo { get; set; } = null!;

    /// <summary>Existencia actual en inventario.</summary>
    public decimal stock { get; set; }

    /// <summary>Existencia minima deseada.</summary>
    public decimal stock_min { get; set; }

    /// <summary>Existencia maxima deseada.</summary>
    public decimal stock_max { get; set; }

    /// <summary>Tasa de IVA aplicable al articulo.</summary>
    public decimal iva { get; set; }

    /// <summary>Fecha de inicio de vigencia como kit (opcional).</summary>
    public DateTime? kit_fecha_ini { get; set; }

    /// <summary>Fecha de fin de vigencia como kit (opcional).</summary>
    public DateTime? kit_fecha_fin { get; set; }

    /// <summary>Indica si el articulo esta disponible para venta.</summary>
    public bool articulo_disponible { get; set; }

    /// <summary>Indica si el articulo es un kit/paquete.</summary>
    public bool kit { get; set; }

    /// <summary>Fecha de alta del articulo.</summary>
    public DateTime fecha_registro { get; set; }

    /// <summary>Indica si el articulo es visible en caja.</summary>
    public bool visible { get; set; }

    /// <summary>Puntos de lealtad otorgados por el articulo.</summary>
    public short puntos { get; set; }

    /// <summary>Ultima actualizacion de inventario.</summary>
    public DateTime last_update_inventory { get; set; }

    /// <summary>Clave de producto.</summary>
    public string cve_producto { get; set; } = null!;

    /// <summary>Identificador del objeto de impuesto (opcional).</summary>
    public string? id_objeto_impuesto { get; set; }

    /// <summary>Renglones de venta donde aparece este articulo (relacion 1 a N).</summary>
    public virtual ICollection<VentaArticulo> VentaArticulos { get; set; } = new List<VentaArticulo>();
}
