namespace McpVentas.Application.Dtos;

/// <summary>
/// Desglose de importes por forma de pago. Cada fila corresponde a una de las
/// columnas monetarias de pago de la tabla <c>venta</c>.
/// </summary>
/// <param name="FormaPago">Nombre legible de la forma de pago (p. ej. "Efectivo").</param>
/// <param name="Columna">Nombre de la columna de origen (p. ej. "pago_efectivo").</param>
/// <param name="Importe">Importe total cobrado por esa forma de pago.</param>
/// <param name="Porcentaje">Porcentaje que representa respecto al total cobrado.</param>
public sealed record VentaPorFormaPagoDto(
    string FormaPago,
    string Columna,
    decimal Importe,
    decimal Porcentaje);
