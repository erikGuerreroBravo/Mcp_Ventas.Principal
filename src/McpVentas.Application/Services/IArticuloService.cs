using McpVentas.Application.Dtos;

namespace McpVentas.Application.Services;

/// <summary>
/// Casos de uso relacionados con articulos e inventario.
/// </summary>
public interface IArticuloService
{
    /// <summary>Articulos mas vendidos en un periodo.</summary>
    Task<IReadOnlyList<ArticuloVendidoDto>> TopArticulosVendidosAsync(
        DateTime? desde,
        DateTime? hasta,
        int top,
        CancellationToken ct);

    /// <summary>Articulos con stock en o por debajo del minimo.</summary>
    Task<IReadOnlyList<ArticuloStockDto>> ArticulosBajoStockAsync(int limite, CancellationToken ct);

    /// <summary>Busqueda de articulos por texto y/o codigo de barras.</summary>
    Task<IReadOnlyList<ArticuloDto>> BuscarArticulosAsync(
        string? texto,
        string? codBarras,
        int limite,
        CancellationToken ct);
}
