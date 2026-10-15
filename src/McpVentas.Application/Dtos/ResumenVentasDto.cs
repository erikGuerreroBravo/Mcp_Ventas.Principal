namespace McpVentas.Application.Dtos;

/// <summary>
/// Resumen agregado de ventas para un periodo. Es el resultado de sumar
/// las columnas monetarias de la tabla <c>venta</c>.
/// </summary>
/// <param name="Desde">Fecha inicial del periodo analizado (inclusive).</param>
/// <param name="Hasta">Fecha final del periodo analizado (inclusive).</param>
/// <param name="NumeroVentas">Cantidad de tickets/ventas en el periodo.</param>
/// <param name="TotalVendido">Suma de importes cobrados.</param>
/// <param name="Subtotal">Suma de subtotales.</param>
/// <param name="Impuestos">Suma de impuestos.</param>
/// <param name="Descuento">Suma de descuentos.</param>
/// <param name="TicketPromedio">Total dividido entre el numero de ventas.</param>
public sealed record ResumenVentasDto(
    DateTime Desde,
    DateTime Hasta,
    int NumeroVentas,
    decimal TotalVendido,
    decimal Subtotal,
    decimal Impuestos,
    decimal Descuento,
    decimal TicketPromedio);
