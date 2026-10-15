using Microsoft.Extensions.Logging;
using McpVentas.Application.Dtos;
using McpVentas.Application.Abstractions;

namespace McpVentas.Application.Services;

/// <summary>
/// Implementacion de los casos de uso de articulos e inventario.
/// Valida y normaliza parametros, traza la ejecucion y delega en el repositorio.
/// </summary>
public sealed class ArticuloService : IArticuloService
{
    // Limite maximo de resultados para no saturar la respuesta.
    private const int LimiteMaximo = 200;

    // Puerto de acceso a datos de articulos.
    private readonly IArticuloRepository _repo;
    // Logger tipado del servicio.
    private readonly ILogger<ArticuloService> _logger;

    /// <summary>Constructor con inyeccion de dependencias.</summary>
    public ArticuloService(IArticuloRepository repo, ILogger<ArticuloService> logger)
    {
        // Guarda el repositorio.
        _repo = repo;
        // Guarda el logger.
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<ArticuloVendidoDto>> TopArticulosVendidosAsync(
        DateTime? desde,
        DateTime? hasta,
        int top,
        CancellationToken ct)
    {
        // Valida que el rango de fechas sea coherente.
        if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
        {
            // Error de negocio: rango invertido.
            throw new ArgumentException("La fecha 'desde' no puede ser mayor que 'hasta'.");
        }
        // Normaliza el "top" dentro del rango permitido.
        var topSeguro = Math.Clamp(top, 1, LimiteMaximo);
        // Registra la operacion.
        _logger.LogInformation("TopArticulosVendidos desde={Desde} hasta={Hasta} top={Top}", desde, hasta, topSeguro);
        // Delega la consulta al repositorio.
        return _repo.TopVendidosAsync(desde, hasta, topSeguro, ct);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<ArticuloStockDto>> ArticulosBajoStockAsync(int limite, CancellationToken ct)
    {
        // Normaliza el limite dentro del rango permitido.
        var limiteSeguro = Math.Clamp(limite, 1, LimiteMaximo);
        // Registra la operacion.
        _logger.LogInformation("ArticulosBajoStock limite={Limite}", limiteSeguro);
        // Delega la consulta al repositorio.
        return _repo.BajoStockAsync(limiteSeguro, ct);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<ArticuloDto>> BuscarArticulosAsync(
        string? texto,
        string? codBarras,
        int limite,
        CancellationToken ct)
    {
        // Exige al menos un criterio de busqueda.
        if (string.IsNullOrWhiteSpace(texto) && string.IsNullOrWhiteSpace(codBarras))
        {
            // Error de negocio: busqueda vacia.
            throw new ArgumentException("Debe indicar 'texto' o 'codBarras' para buscar.");
        }
        // Normaliza el limite dentro del rango permitido.
        var limiteSeguro = Math.Clamp(limite, 1, LimiteMaximo);
        // Registra la operacion.
        _logger.LogInformation("BuscarArticulos texto={Texto} codBarras={CodBarras} limite={Limite}", texto, codBarras, limiteSeguro);
        // Delega la consulta al repositorio.
        return _repo.BuscarAsync(texto, codBarras, limiteSeguro, ct);
    }
}
