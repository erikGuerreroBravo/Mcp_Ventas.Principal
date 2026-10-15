using System;
using System.Collections.Generic;

// Namespace de las entidades generadas por la ingenieria inversa (Database-First).
namespace McpVentas.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad que representa la tabla <c>venta</c> (cabecera de cada ticket).
/// Los nombres de las propiedades conservan el nombre fisico de la columna
/// para que EF Core las mapee por convencion sin configuracion adicional.
/// Es una clase <c>partial</c> para permitir extenderla sin tocar este archivo generado.
/// </summary>
public partial class Venta
{
    /// <summary>Identificador de la caja/terminal punto de venta. Parte de la PK.</summary>
    public int id_pos { get; set; }

    /// <summary>Identificador unico de la venta (GUID). Parte de la PK.</summary>
    public Guid id_venta { get; set; }

    /// <summary>Nombre de usuario del vendedor que atendio la venta.</summary>
    public string vendedor { get; set; } = null!;

    /// <summary>Folio secuencial de la venta.</summary>
    public long folio { get; set; }

    /// <summary>Fecha y hora en que se registro la venta.</summary>
    public DateTime fecha_venta { get; set; }

    /// <summary>Importe total cobrado en la venta.</summary>
    public decimal total_vendido { get; set; }

    /// <summary>Importe pagado en efectivo.</summary>
    public decimal pago_efectivo { get; set; }

    /// <summary>Importe pagado con cheque.</summary>
    public decimal pago_cheque { get; set; }

    /// <summary>Importe pagado con vales de despensa.</summary>
    public decimal pago_vales { get; set; }

    /// <summary>Importe pagado con tarjeta de credito.</summary>
    public decimal pago_tc { get; set; }

    /// <summary>Usuario del supervisor que autorizo la venta (opcional).</summary>
    public string? supervisor { get; set; }

    /// <summary>Bandera de sincronizacion/subida de la venta al sistema central.</summary>
    public bool upload { get; set; }

    /// <summary>Cantidad de renglones (articulos) registrados en la venta.</summary>
    public short num_registros { get; set; }

    /// <summary>Importe pagado con tarjeta de debito.</summary>
    public decimal pago_td { get; set; }

    /// <summary>Importe pagado por transferencia SPEI.</summary>
    public decimal pago_spei { get; set; }

    /// <summary>Subtotal de la venta antes de impuestos.</summary>
    public decimal subtotal { get; set; }

    /// <summary>IVA desglosado de la venta.</summary>
    public decimal iva_desglosado { get; set; }

    /// <summary>IEPS desglosado de la venta.</summary>
    public decimal ieps_desglosado { get; set; }

    /// <summary>Impuestos totales (IVA + IEPS).</summary>
    public decimal impuestos { get; set; }

    /// <summary>Descuento total aplicado a la venta.</summary>
    public decimal descuento { get; set; }

    /// <summary>Renglones de articulo que pertenecen a esta venta (relacion 1 a N).</summary>
    public virtual ICollection<VentaArticulo> Lineas { get; set; } = new List<VentaArticulo>();
}
