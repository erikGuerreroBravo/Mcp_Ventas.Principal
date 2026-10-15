namespace McpVentas.Application.Dtos;

/// <summary>
/// Linea (renglon) de detalle de una venta, combinando datos de
/// <c>venta_articulo</c> con la descripcion de <c>articulo</c>.
/// </summary>
/// <param name="CodBarras">Codigo de barras del articulo.</param>
/// <param name="Descripcion">Descripcion comercial del articulo.</param>
/// <param name="Cantidad">Cantidad vendida.</param>
/// <param name="PrecioVenta">Precio unitario de venta con el que se cobro.</param>
/// <param name="Importe">Importe de la linea (cantidad por precio menos descuento).</param>
/// <param name="Iva">IVA aplicado a la linea.</param>
/// <param name="Descuento">Porcentaje de descuento aplicado a la linea.</param>
/// <param name="Devuelta">Cantidad devuelta de este articulo.</param>
public sealed record VentaDetalleDto(
    string CodBarras,
    string Descripcion,
    decimal Cantidad,
    decimal PrecioVenta,
    decimal Importe,
    decimal Iva,
    decimal Descuento,
    decimal Devuelta);
