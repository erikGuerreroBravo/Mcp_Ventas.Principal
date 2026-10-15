namespace McpVentas.Application.Dtos;

/// <summary>
/// Articulo con su situacion de inventario (tabla <c>articulo</c>).
/// </summary>
/// <param name="CodBarras">Codigo de barras (clave primaria de <c>articulo</c>).</param>
/// <param name="Descripcion">Descripcion comercial.</param>
/// <param name="DescripcionCorta">Descripcion corta para tickets.</param>
/// <param name="PrecioVenta">Precio de venta actual.</param>
/// <param name="PrecioCompra">Precio de compra actual.</param>
/// <param name="Stock">Existencia actual.</param>
/// <param name="StockMin">Existencia minima deseada.</param>
/// <param name="StockMax">Existencia maxima deseada.</param>
/// <param name="TipoArticulo">Clasificacion del articulo.</param>
/// <param name="Iva">IVA aplicable.</param>
public sealed record ArticuloDto(
    string CodBarras,
    string Descripcion,
    string DescripcionCorta,
    decimal PrecioVenta,
    decimal PrecioCompra,
    decimal Stock,
    decimal StockMin,
    decimal StockMax,
    string TipoArticulo,
    decimal Iva);
