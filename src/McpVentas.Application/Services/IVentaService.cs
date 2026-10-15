using McpVentas.Application.Dtos;

namespace McpVentas.Application.Services;

/// <summary>
/// Casos de uso de consulta y analitica sobre ventas.
/// Es la capa que consumen directamente las herramientas MCP (adaptadores de transporte).
/// </summary>
public interface IVentaService
{
    /// <summary>Lista ventas paginadas aplicando filtros opcionales.</summary>
    Task<PaginaDto<VentaDto>> ListarVentasAsync(
        DateTime? desde,
        DateTime? hasta,
        string? vendedor,
        long? folio,
        int pagina,
        int tamanio,
        CancellationToken ct);

    /// <summary>Recupera la cabecera de una venta por folio.</summary>
    Task<VentaDto?> ObtenerVentaPorFolioAsync(long folio, CancellationToken ct);

    /// <summary>Recupera el detalle de articulos de una venta por folio o por id.</summary>
    Task<IReadOnlyList<VentaDetalleDto>> ObtenerDetalleAsync(
        long? folio,
        Guid? idVenta,
        CancellationToken ct);

    /// <summary>Resumen agregado del periodo.</summary>
    Task<ResumenVentasDto> ResumenPeriodoAsync(DateTime? desde, DateTime? hasta, CancellationToken ct);

    /// <summary>Ranking de vendedores en el periodo.</summary>
    Task<IReadOnlyList<VentaPorVendedorDto>> VentasPorVendedorAsync(
        DateTime? desde,
        DateTime? hasta,
        int limite,
        CancellationToken ct);

    /// <summary>Serie temporal diaria de ventas.</summary>
    Task<IReadOnlyList<VentaPorDiaDto>> VentasPorDiaAsync(
        DateTime? desde,
        DateTime? hasta,
        int limite,
        CancellationToken ct);

    /// <summary>Desglose por forma de pago.</summary>
    Task<IReadOnlyList<VentaPorFormaPagoDto>> VentasPorFormaPagoAsync(
        DateTime? desde,
        DateTime? hasta,
        CancellationToken ct);
}
