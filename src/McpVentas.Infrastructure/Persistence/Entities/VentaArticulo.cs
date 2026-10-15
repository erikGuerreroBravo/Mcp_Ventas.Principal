using System;
using System.Collections.Generic;

namespace McpVentas.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad que representa la tabla <c>venta_articulo</c> (renglones del ticket).
/// La clave primaria es compuesta: (no_articulo, id_pos, id_venta).
/// Los nombres de propiedades conservan el nombre fisico de la columna.
/// </summary>
public partial class VentaArticulo
{
    /// <summary>Identificador de la caja/terminal. Parte de la PK compuesta.</summary>
    public int id_pos { get; set; }

    /// <summary>Identificador de la venta a la que pertenece el renglon. Parte de la PK compuesta.</summary>
    public Guid id_venta { get; set; }

    /// <summary>Numero de renglon dentro de la venta. Parte de la PK compuesta.</summary>
    public long no_articulo { get; set; }

    /// <summary>Codigo de barras del articulo vendido (FK a Articulo).</summary>
    public string cod_barras { get; set; } = null!;

    /// <summary>Codigo de usuario de bascula (opcional, para articulos a granel).</summary>
    public short? user_code_bascula { get; set; }

    /// <summary>Cantidad vendida del articulo.</summary>
    public decimal cantidad { get; set; }

    /// <summary>Indica si el articulo fue ofertado/promocionado.</summary>
    public bool articulo_ofertado { get; set; }

    /// <summary>Precio regular del articulo antes de cualquier cambio.</summary>
    public decimal precio_regular { get; set; }

    /// <summary>Indica si el precio fue modificado manualmente en caja.</summary>
    public bool cambio_precio { get; set; }

    /// <summary>Tasa de IVA aplicada a la linea.</summary>
    public decimal iva { get; set; }

    /// <summary>Precio unitario de venta con el que se cobro la linea.</summary>
    public decimal precio_vta { get; set; }

    /// <summary>Porcentaje de descuento aplicado a la linea.</summary>
    public decimal porcent_desc { get; set; }

    /// <summary>Cantidad devuelta del articulo.</summary>
    public decimal cant_devuelta { get; set; }

    /// <summary>Usuario que registro el renglon (opcional).</summary>
    public string? user_name { get; set; }

    /// <summary>Identificador de la promocion aplicada (opcional).</summary>
    public Guid? id_promo { get; set; }

    /// <summary>Numero de promocion aplicada (opcional).</summary>
    public short? no_promo_aplicado { get; set; }

    /// <summary>Tasa de IEPS aplicada a la linea (opcional).</summary>
    public decimal? ieps { get; set; }

    /// <summary>Precio de compra del articulo al momento de la venta.</summary>
    public decimal precio_compra { get; set; }

    /// <summary>Utilidad generada por la linea (precio de venta menos compra por cantidad).</summary>
    public decimal utilidad { get; set; }

    /// <summary>Navegacion al articulo (FK por <c>cod_barras</c>).</summary>
    public virtual Articulo Articulo { get; set; } = null!;

    /// <summary>Navegacion a la venta (FK por <c>id_venta</c> + <c>id_pos</c>).</summary>
    public virtual Venta Venta { get; set; } = null!;
}
