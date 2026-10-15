// Importa la abstraccion de logging para poder registrar trazas del caso de uso.
using Microsoft.Extensions.Logging;
// Importa los DTOs de salida.
using McpVentas.Application.Dtos;
// Importa los contratos (puertos) del repositorio.
using McpVentas.Application.Abstractions;

namespace McpVentas.Application.Services;

/// <summary>
/// Implementacion de los casos de uso de ventas. Aplica validaciones y
/// normalizacion de parametros, registra trazas y delega la lectura de datos
/// al repositorio (<see cref="IVentaRepository"/>). No conoce EF Core ni SQL.
/// </summary>
public sealed class VentaService : IVentaService
{
    // Limite maximo de elementos por pagina para proteger al servidor y al LLM.
    private const int TamanioPaginaMaximo = 200;
    // Limite maximo de filas para resultados de ranking/series.
    private const int LimiteMaximo = 500;

    // Puerto de acceso a datos, inyectado por el contenedor de dependencias.
    private readonly IVentaRepository _repo;
    // Logger tipado para trazar la ejecucion del caso de uso.
    private readonly ILogger<VentaService> _logger;

    /// <summary>Constructor que recibe sus dependencias por inyeccion.</summary>
    public VentaService(IVentaRepository repo, ILogger<VentaService> logger)
    {
        // Guarda el repositorio para usarlo en los metodos.
        _repo = repo;
        // Guarda el logger para usarlo en los metodos.
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<PaginaDto<VentaDto>> ListarVentasAsync(
        DateTime? desde,
        DateTime? hasta,
        string? vendedor,
        long? folio,
        int pagina,
        int tamanio,
        CancellationToken ct)
    {
        // Normaliza la pagina a un valor valido (>= 1).
        var paginaSegura = pagina < 1 ? 1 : pagina;
        // Normaliza el tamanio de pagina dentro del rango permitido.
        var tamanioSeguro = Math.Clamp(tamanio, 1, TamanioPaginaMaximo);
        // Valida que el rango de fechas sea coherente.
        ValidarRango(desde, hasta);
        // Registra la operacion a nivel de informacion.
        _logger.LogInformation(
            "ListarVentas desde={Desde} hasta={Hasta} vendedor={Vendedor} folio={Folio} pagina={Pagina} tamanio={Tamanio}",
            desde, hasta, vendedor, folio, paginaSegura, tamanioSeguro);
        // Delega la consulta al repositorio.
        return _repo.ListarAsync(desde, hasta, vendedor, folio, paginaSegura, tamanioSeguro, ct);
    }

    /// <inheritdoc />
    public Task<VentaDto?> ObtenerVentaPorFolioAsync(long folio, CancellationToken ct)
    {
        // Registra la busqueda puntual.
        _logger.LogInformation("ObtenerVentaPorFolio folio={Folio}", folio);
        // Delega la consulta al repositorio.
        return _repo.ObtenerPorFolioAsync(folio, ct);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<VentaDetalleDto>> ObtenerDetalleAsync(
        long? folio,
        Guid? idVenta,
        CancellationToken ct)
    {
        // Exige que se indique al menos uno de los dos identificadores.
        if (folio is null && idVenta is null)
        {
            // Lanza un error claro que la herramienta MCP devolvera al cliente.
            throw new ArgumentException("Debe indicar 'folio' o 'idVenta' para obtener el detalle.");
        }
        // Registra la operacion.
        _logger.LogInformation("ObtenerDetalle folio={Folio} idVenta={IdVenta}", folio, idVenta);
        // Delega la consulta al repositorio.
        return _repo.ObtenerDetalleAsync(folio, idVenta, ct);
    }

    /// <inheritdoc />
    public Task<ResumenVentasDto> ResumenPeriodoAsync(DateTime? desde, DateTime? hasta, CancellationToken ct)
    {
        // Valida el rango de fechas.
        ValidarRango(desde, hasta);
        // Registra la operacion de agregacion.
        _logger.LogInformation("ResumenPeriodo desde={Desde} hasta={Hasta}", desde, hasta);
        // Delega la consulta al repositorio.
        return _repo.ResumenPeriodoAsync(desde, hasta, ct);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<VentaPorVendedorDto>> VentasPorVendedorAsync(
        DateTime? desde,
        DateTime? hasta,
        int limite,
        CancellationToken ct)
    {
        // Valida el rango de fechas.
        ValidarRango(desde, hasta);
        // Normaliza el limite dentro del rango permitido.
        var limiteSeguro = Math.Clamp(limite, 1, LimiteMaximo);
        // Registra la operacion.
        _logger.LogInformation("VentasPorVendedor desde={Desde} hasta={Hasta} limite={Limite}", desde, hasta, limiteSeguro);
        // Delega la consulta al repositorio.
        return _repo.PorVendedorAsync(desde, hasta, limiteSeguro, ct);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<VentaPorDiaDto>> VentasPorDiaAsync(
        DateTime? desde,
        DateTime? hasta,
        int limite,
        CancellationToken ct)
    {
        // Valida el rango de fechas.
        ValidarRango(desde, hasta);
        // Normaliza el limite dentro del rango permitido.
        var limiteSeguro = Math.Clamp(limite, 1, LimiteMaximo);
        // Registra la operacion.
        _logger.LogInformation("VentasPorDia desde={Desde} hasta={Hasta} limite={Limite}", desde, hasta, limiteSeguro);
        // Delega la consulta al repositorio.
        return _repo.PorDiaAsync(desde, hasta, limiteSeguro, ct);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<VentaPorFormaPagoDto>> VentasPorFormaPagoAsync(
        DateTime? desde,
        DateTime? hasta,
        CancellationToken ct)
    {
        // Valida el rango de fechas.
        ValidarRango(desde, hasta);
        // Registra la operacion.
        _logger.LogInformation("VentasPorFormaPago desde={Desde} hasta={Hasta}", desde, hasta);
        // Delega la consulta al repositorio.
        return _repo.PorFormaPagoAsync(desde, hasta, ct);
    }

    /// <summary>
    /// Valida que la fecha inicial no sea posterior a la final.
    /// </summary>
    private static void ValidarRango(DateTime? desde, DateTime? hasta)
    {
        // Solo valida cuando ambas fechas fueron proporcionadas.
        if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
        {
            // Error de negocio: rango invertido.
            throw new ArgumentException("La fecha 'desde' no puede ser mayor que 'hasta'.");
        }
    }
}
