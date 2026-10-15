// Las interfaces de repositorio son "puertos" que Application define y que
// Infrastructure implementa. Esto invierte la dependencia (Dependency Inversion):
// Application no conoce EF Core ni SQL Server, solo el contrato.
using McpVentas.Application.Dtos;

namespace McpVentas.Application.Abstractions;

/// <summary>
/// Contrato de solo lectura para consultar la tabla <c>venta</c> y su detalle.
/// Todas las operaciones son asincronas y cancelables.
/// </summary>
public interface IVentaRepository
{
    /// <summary>
    /// Lista ventas con filtros opcionales y paginacion.
    /// </summary>
    /// <param name="desde">Fecha inicial (inclusive) o <c>null</c> para no filtrar.</param>
    /// <param name="hasta">Fecha final (inclusive) o <c>null</c> para no filtrar.</param>
    /// <param name="vendedor">Usuario de vendedor exacto o <c>null</c>.</param>
    /// <param name="folio">Folio exacto o <c>null</c>.</param>
    /// <param name="pagina">Pagina solicitada (1-based).</param>
    /// <param name="tamanio">Elementos por pagina.</param>
    /// <param name="ct">Token de cancelacion.</param>
    Task<PaginaDto<VentaDto>> ListarAsync(
        DateTime? desde,
        DateTime? hasta,
        string? vendedor,
        long? folio,
        int pagina,
        int tamanio,
        CancellationToken ct);

    /// <summary>Obtiene la cabecera de una venta por su folio, o <c>null</c> si no existe.</summary>
    Task<VentaDto?> ObtenerPorFolioAsync(long folio, CancellationToken ct);

    /// <summary>Obtiene el detalle de una venta por folio o por <c>id_venta</c>.</summary>
    Task<IReadOnlyList<VentaDetalleDto>> ObtenerDetalleAsync(
        long? folio,
        Guid? idVenta,
        CancellationToken ct);

    /// <summary>Calcula el resumen agregado de un periodo.</summary>
    Task<ResumenVentasDto> ResumenPeriodoAsync(
        DateTime? desde,
        DateTime? hasta,
        CancellationToken ct);

    /// <summary>Agrupa las ventas por vendedor, ordenadas por importe descendente.</summary>
    Task<IReadOnlyList<VentaPorVendedorDto>> PorVendedorAsync(
        DateTime? desde,
        DateTime? hasta,
        int limite,
        CancellationToken ct);

    /// <summary>Genera la serie temporal diaria de ventas.</summary>
    Task<IReadOnlyList<VentaPorDiaDto>> PorDiaAsync(
        DateTime? desde,
        DateTime? hasta,
        int limite,
        CancellationToken ct);

    /// <summary>Desglosa los importes cobrados por forma de pago.</summary>
    Task<IReadOnlyList<VentaPorFormaPagoDto>> PorFormaPagoAsync(
        DateTime? desde,
        DateTime? hasta,
        CancellationToken ct);
}
