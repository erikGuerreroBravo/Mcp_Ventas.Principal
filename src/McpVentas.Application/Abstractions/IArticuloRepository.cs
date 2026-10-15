using McpVentas.Application.Dtos;

namespace McpVentas.Application.Abstractions;

/// <summary>
/// Contrato de solo lectura para consultar articulos e inventario
/// (tablas <c>articulo</c> y <c>venta_articulo</c>).
/// </summary>
public interface IArticuloRepository
{
    /// <summary>Devuelve los articulos mas vendidos (por cantidad) en un periodo.</summary>
    Task<IReadOnlyList<ArticuloVendidoDto>> TopVendidosAsync(
        DateTime? desde,
        DateTime? hasta,
        int top,
        CancellationToken ct);

    /// <summary>Devuelve los articulos con stock igual o inferior al minimo.</summary>
    Task<IReadOnlyList<ArticuloStockDto>> BajoStockAsync(
        int limite,
        CancellationToken ct);

    /// <summary>
    /// Busca articulos por texto libre (descripcion) y/o por codigo de barras exacto.
    /// </summary>
    Task<IReadOnlyList<ArticuloDto>> BuscarAsync(
        string? texto,
        string? codBarras,
        int limite,
        CancellationToken ct);
}
