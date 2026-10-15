// Importa el atributo [Description] para documentar tools y parametros al LLM.
using System.ComponentModel;
// Importa McpException (errores de tool que el cliente/LLM puede leer).
using ModelContextProtocol;
// Importa los atributos de definicion de herramientas MCP.
using ModelContextProtocol.Server;
// Importa la abstraccion de logging.
using Microsoft.Extensions.Logging;
// Importa los casos de uso de ventas.
using McpVentas.Application.Services;

namespace McpVentas.Server.Tools;

/// <summary>
/// Herramientas MCP de analitica de ventas. Cada metodo publico marcado con
/// <see cref="McpServerToolAttribute"/> se expone automaticamente al cliente MCP
/// (registro automatico via WithToolsFromAssembly). Las dependencias se inyectan
/// por constructor (DI nativa).
/// </summary>
[McpServerToolType]
public sealed class VentaTools
{
    // Caso de uso de ventas (inyectado).
    private readonly IVentaService _ventas;
    // Logger de la herramienta (inyectado).
    private readonly ILogger<VentaTools> _logger;

    /// <summary>Constructor con inyeccion de dependencias.</summary>
    public VentaTools(IVentaService ventas, ILogger<VentaTools> logger)
    {
        // Guarda el servicio de ventas.
        _ventas = ventas;
        // Guarda el logger.
        _logger = logger;
    }

    /// <summary>Ejecuta una funcion de consulta y traduce errores de negocio a McpException.</summary>
    private async Task<string> EjecutarAsync(string tool, Func<Task<object>> accion)
    {
        // Traza el inicio de la invocacion de la herramienta.
        _logger.LogInformation("Ejecutando tool MCP '{Tool}'", tool);
        try
        {
            // Ejecuta la accion y serializa el resultado a JSON.
            var resultado = await accion();
            return ToolJson.Serializar(resultado);
        }
        catch (ArgumentException ex)
        {
            // Los errores de validacion se devuelven al LLM con su mensaje real.
            _logger.LogWarning(ex, "Error de validacion en tool '{Tool}'", tool);
            throw new McpException(ex.Message);
        }
        catch (Exception ex)
        {
            // Cualquier otro error se registra y se informa al LLM.
            _logger.LogError(ex, "Error inesperado en tool '{Tool}'", tool);
            throw new McpException("Ocurrio un error al ejecutar la consulta solicitada.");
        }
    }

    /// <summary>Lista ventas con filtros opcionales y paginacion.</summary>
    [McpServerTool(Name = "listar_ventas")]
    [Description("Lista las ventas registradas en el POS con filtros opcionales de fecha, vendedor y folio, paginadas. Devuelve cabeceras de ticket (sin el detalle de articulos).")]
    public Task<string> ListarVentasAsync(
        [Description("Fecha inicial del rango (formato ISO, p.ej. 2023-01-01). Opcional.")] DateTime? fechaDesde = null,
        [Description("Fecha final del rango (formato ISO, p.ej. 2023-12-31). Opcional.")] DateTime? fechaHasta = null,
        [Description("Nombre de usuario exacto del vendedor. Opcional.")] string? vendedor = null,
        [Description("Folio exacto de la venta. Opcional.")] long? folio = null,
        [Description("Numero de pagina (1-based). Por defecto 1.")] int pagina = 1,
        [Description("Cantidad de ventas por pagina (1-200). Por defecto 50.")] int tamanio = 50,
        CancellationToken ct = default)
        // Delega validacion/normalizacion al servicio y serializa el resultado.
        => EjecutarAsync(nameof(ListarVentasAsync), async () =>
            await _ventas.ListarVentasAsync(fechaDesde, fechaHasta, vendedor, folio, pagina, tamanio, ct));

    /// <summary>Obtiene la cabecera de una venta por folio.</summary>
    [McpServerTool(Name = "obtener_venta_por_folio")]
    [Description("Obtiene la cabecera de una venta (totales, impuestos, formas de pago) buscandola por su folio.")]
    public Task<string> ObtenerVentaPorFolioAsync(
        [Description("Folio de la venta a consultar.")] long folio,
        CancellationToken ct = default)
        // Busca la venta y devuelve null serializado si no existe.
        => EjecutarAsync(nameof(ObtenerVentaPorFolioAsync), async () =>
            await _ventas.ObtenerVentaPorFolioAsync(folio, ct));

    /// <summary>Obtiene el detalle (renglones) de una venta.</summary>
    [McpServerTool(Name = "obtener_detalle_venta")]
    [Description("Obtiene el detalle de articulos (renglones) de una venta, indicando el folio o el id_venta (GUID) de la venta.")]
    public Task<string> ObtenerDetalleVentaAsync(
        [Description("Folio de la venta. Opcional si se proporciona idVenta.")] long? folio = null,
        [Description("Identificador GUID de la venta. Opcional si se proporciona folio.")] Guid? idVenta = null,
        CancellationToken ct = default)
        // El servicio valida que al menos uno de los dos identificadores venga.
        => EjecutarAsync(nameof(ObtenerDetalleVentaAsync), async () =>
            await _ventas.ObtenerDetalleAsync(folio, idVenta, ct));

    /// <summary>Resumen agregado de un periodo.</summary>
    [McpServerTool(Name = "resumen_ventas_periodo")]
    [Description("Calcula el resumen de ventas de un periodo: numero de ventas, total vendido, subtotal, impuestos, descuento y ticket promedio.")]
    public Task<string> ResumenVentasPeriodoAsync(
        [Description("Fecha inicial del periodo (ISO). Opcional; sin valor abarca todo el historial.")] DateTime? fechaDesde = null,
        [Description("Fecha final del periodo (ISO). Opcional; sin valor abarca todo el historial.")] DateTime? fechaHasta = null,
        CancellationToken ct = default)
        // Delega el calculo al servicio.
        => EjecutarAsync(nameof(ResumenVentasPeriodoAsync), async () =>
            await _ventas.ResumenPeriodoAsync(fechaDesde, fechaHasta, ct));

    /// <summary>Ranking de ventas por vendedor.</summary>
    [McpServerTool(Name = "ventas_por_vendedor")]
    [Description("Agrupa las ventas por vendedor en un periodo y las ordena por importe vendido descendente. Incluye nombre completo del empleado cuando existe.")]
    public Task<string> VentasPorVendedorAsync(
        [Description("Fecha inicial del periodo (ISO). Opcional.")] DateTime? fechaDesde = null,
        [Description("Fecha final del periodo (ISO). Opcional.")] DateTime? fechaHasta = null,
        [Description("Numero maximo de vendedores a devolver (1-500). Por defecto 20.")] int limite = 20,
        CancellationToken ct = default)
        // Delega el calculo al servicio.
        => EjecutarAsync(nameof(VentasPorVendedorAsync), async () =>
            await _ventas.VentasPorVendedorAsync(fechaDesde, fechaHasta, limite, ct));

    /// <summary>Serie temporal diaria de ventas.</summary>
    [McpServerTool(Name = "ventas_por_dia")]
    [Description("Devuelve la serie diaria de ventas (numero de ventas e importe por dia) para dibujar tendencias en un periodo.")]
    public Task<string> VentasPorDiaAsync(
        [Description("Fecha inicial del periodo (ISO). Opcional.")] DateTime? fechaDesde = null,
        [Description("Fecha final del periodo (ISO). Opcional.")] DateTime? fechaHasta = null,
        [Description("Numero maximo de dias a devolver (los mas recientes). Por defecto 90.")] int limite = 90,
        CancellationToken ct = default)
        // Delega el calculo al servicio.
        => EjecutarAsync(nameof(VentasPorDiaAsync), async () =>
            await _ventas.VentasPorDiaAsync(fechaDesde, fechaHasta, limite, ct));

    /// <summary>Desglose por forma de pago.</summary>
    [McpServerTool(Name = "ventas_por_forma_pago")]
    [Description("Desglosa los importes cobrados por forma de pago (efectivo, cheque, vales, tarjeta de credito/debito, SPEI) e indica el porcentaje de cada una.")]
    public Task<string> VentasPorFormaPagoAsync(
        [Description("Fecha inicial del periodo (ISO). Opcional.")] DateTime? fechaDesde = null,
        [Description("Fecha final del periodo (ISO). Opcional.")] DateTime? fechaHasta = null,
        CancellationToken ct = default)
        // Delega el calculo al servicio.
        => EjecutarAsync(nameof(VentasPorFormaPagoAsync), async () =>
            await _ventas.VentasPorFormaPagoAsync(fechaDesde, fechaHasta, ct));
}
