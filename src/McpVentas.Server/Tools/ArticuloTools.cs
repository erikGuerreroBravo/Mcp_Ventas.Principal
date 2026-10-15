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
/// Herramientas MCP de catalogo e inventario de articulos.
/// </summary>
[McpServerToolType]
public sealed class ArticuloTools
{
    // Caso de uso de articulos (inyectado).
    private readonly IArticuloService _articulos;
    // Logger (inyectado).
    private readonly ILogger<ArticuloTools> _logger;

    /// <summary>Constructor con inyeccion de dependencias.</summary>
    public ArticuloTools(IArticuloService articulos, ILogger<ArticuloTools> logger)
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

    /// <summary>Lista articulos con stock igual o inferior al minimo.</summary>
    [McpServerTool(Name = "articulos_bajo_stock")]
    [Description("Devuelve los articulos cuyo stock esta en o por debajo del minimo configurado, con la cantidad faltante sugerida para reabastecer.")]
    public Task<string> ArticulosBajoStockAsync(
        [Description("Cantidad maxima de articulos a devolver (1-200). Por defecto 50.")] int limite = 50,
        CancellationToken ct = default)
        // Delega la consulta al servicio de articulos.
        => EjecutarAsync(nameof(ArticulosBajoStockAsync), async () =>
            await _articulos.ArticulosBajoStockAsync(limite, ct));

    /// <summary>Busca articulos por texto o por codigo de barras.</summary>
    [McpServerTool(Name = "buscar_articulo")]
    [Description("Busca articulos del catalogo por texto libre en la descripcion y/o por codigo de barras exacto. Devuelve precio, stock e impuestos.")]
    public Task<string> BuscarArticuloAsync(
        [Description("Texto a buscar en la descripcion del articulo. Opcional si se indica codBarras.")] string? texto = null,
        [Description("Codigo de barras exacto del articulo. Opcional si se indica texto.")] string? codBarras = null,
        [Description("Cantidad maxima de resultados (1-200). Por defecto 50.")] int limite = 50,
        CancellationToken ct = default)
        // El servicio valida que al menos un criterio venga informado.
        => EjecutarAsync(nameof(BuscarArticuloAsync), async () =>
            await _articulos.BuscarArticulosAsync(texto, codBarras, limite, ct));
}
