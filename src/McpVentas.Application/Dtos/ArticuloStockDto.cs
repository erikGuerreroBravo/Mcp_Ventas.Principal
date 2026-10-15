namespace McpVentas.Application.Dtos;

/// <summary>
/// Articulo cuyo stock esta en o por debajo del minimo configurado.
/// Sirve para que el LLM sugiera reposicion de inventario.
/// </summary>
/// <param name="CodBarras">Codigo de barras del articulo.</param>
/// <param name="Descripcion">Descripcion comercial.</param>
/// <param name="Stock">Existencia actual.</param>
/// <param name="StockMin">Existencia minima deseada.</param>
/// <param name="StockMax">Existencia maxima deseada.</param>
/// <param name="Faltante">Cantidad sugerida para reabastecer (StockMax - Stock).</param>
/// <param name="PrecioVenta">Precio de venta actual.</param>
public sealed record ArticuloStockDto(
    string CodBarras,
    string Descripcion,
    decimal Stock,
    decimal StockMin,
    decimal StockMax,
    decimal Faltante,
    decimal PrecioVenta);
