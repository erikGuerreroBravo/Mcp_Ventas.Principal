namespace McpVentas.Application.Dtos;

/// <summary>
/// Serie temporal de ventas: un punto por dia. Util para dibujar tendencias.
/// </summary>
/// <param name="Fecha">Dia (solo fecha, sin hora).</param>
/// <param name="NumeroVentas">Cantidad de ventas de ese dia.</param>
/// <param name="TotalVendido">Importe total vendido ese dia.</param>
public sealed record VentaPorDiaDto(
    DateTime Fecha,
    int NumeroVentas,
    decimal TotalVendido);
