namespace McpVentas.Application.Dtos;

/// <summary>
/// Cabecera de una venta (una fila de la tabla <c>venta</c>).
/// Representa el encabezado del ticket; el detalle de articulos va en <see cref="VentaDetalleDto"/>.
/// </summary>
/// <param name="IdVenta">Identificador unico de la venta (GUID).</param>
/// <param name="IdPos">Identificador de la caja/terminal punto de venta.</param>
/// <param name="Folio">Folio secuencial de la venta.</param>
/// <param name="FechaVenta">Fecha y hora en que se registro la venta.</param>
/// <param name="Vendedor">Nombre de usuario del vendedor.</param>
/// <param name="TotalVendido">Importe total cobrado.</param>
/// <param name="Subtotal">Subtotal antes de impuestos.</param>
/// <param name="Impuestos">Impuestos totales (IVA + IEPS).</param>
/// <param name="Descuento">Descuento aplicado.</param>
/// <param name="NumeroRegistros">Cantidad de lineas de articulo de la venta.</param>
public sealed record VentaDto(
    Guid IdVenta,
    int IdPos,
    long Folio,
    DateTime FechaVenta,
    string Vendedor,
    decimal TotalVendido,
    decimal Subtotal,
    decimal Impuestos,
    decimal Descuento,
    short NumeroRegistros);
