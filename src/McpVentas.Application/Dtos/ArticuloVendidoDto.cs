namespace McpVentas.Application.Dtos;

/// <summary>
/// Ranking de articulos mas vendidos en un periodo. Une <c>venta_articulo</c>
/// con <c>articulo</c> para obtener la descripcion.
/// </summary>
/// <param name="CodBarras">Codigo de barras del articulo.</param>
/// <param name="Descripcion">Descripcion del articulo.</param>
/// <param name="CantidadVendida">Suma de cantidades vendidas.</param>
/// <param name="ImporteTotal">Suma de importes vendidos.</param>
/// <param name="UtilidadTotal">Suma de la utilidad generada.</param>
public sealed record ArticuloVendidoDto(
    string CodBarras,
    string Descripcion,
    decimal CantidadVendida,
    decimal ImporteTotal,
    decimal UtilidadTotal);
