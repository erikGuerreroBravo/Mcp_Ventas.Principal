// Importa [Description] para documentar la tool al LLM.
using System.ComponentModel;
// Importa McpException para errores legibles por el cliente.
using ModelContextProtocol;
// Importa los atributos de herramientas MCP.
using ModelContextProtocol.Server;
// Importa la abstraccion de logging.
using Microsoft.Extensions.Logging;
// Importa los casos de uso de articulos.
using McpVentas.Application.Services;

namespace McpVentas.Server.Tools;

/// <summary>
/// Herramientas MCP relacionadas con los articulos vendidos (analitica de producto).
/// </summary>
[McpServerToolType]
public sealed class VentaArticuloTools
{
    // Caso de uso de articulos (inyectado).
    private readonly IArticuloService _articulos;
    // Logger (inyectado).
    private readonly ILogger<VentaArticuloTools> _logger;

    /// <summary>Constructor con inyeccion de dependencias.</summary>
    public VentaArticuloTools(IArticuloService articulos, ILogger<VentaArticuloTools> logger)
    {
        // Guarda el servicio.
        _articulos = articulos;
        // Guarda el logger.
        _logger = logger;
    }

    /// <summary>Ejecuta una consulta y traduce errores de negocio a McpException.</summary>
    private async Task<string> EjecutarAsync(string tool, Func<Task<object>> accion)
    {
        // Traza el inicio.
        _logger.LogInformation("Ejecutando tool MCP '{Tool}'", tool);
        try
        {
            // Ejecuta y serializa a JSON.
            var resultado = await accion();
            return ToolJson.Serializar(resultado);
        }
        catch (ArgumentException ex)
        {
            // Errores de validacion con mensaje real al LLM.
            _logger.LogWarning(ex, "Error de validacion en tool '{Tool}'", tool);
            throw new McpException(ex.Message);
        }
        catch (Exception ex)
        {
            // Errores inesperados.
            _logger.LogError(ex, "Error inesperado en tool '{Tool}'", tool);
            throw new McpException("Ocurrio un error al ejecutar la consulta solicitada.");
        }
    }

    /// <summary>Ranking de articulos mas vendidos.</summary>
    [McpServerTool(Name = "top_articulos_vendidos")]
    [Description("Devuelve los articulos mas vendidos en un periodo, ordenados por cantidad. Incluye cantidad, importe y utilidad totales por articulo.")]
    public Task<string> TopArticulosVendidosAsync(
        [Description("Fecha inicial del periodo (ISO). Opcional.")] DateTime? fechaDesde = null,
        [Description("Fecha final del periodo (ISO). Opcional.")] DateTime? fechaHasta = null,
        [Description("Cantidad de articulos a devolver (1-200). Por defecto 20.")] int top = 20,
        CancellationToken ct = default)
        // Delega el calculo al servicio de articulos.
        => EjecutarAsync(nameof(TopArticulosVendidosAsync), async () =>
            await _articulos.TopArticulosVendidosAsync(fechaDesde, fechaHasta, top, ct));
}
