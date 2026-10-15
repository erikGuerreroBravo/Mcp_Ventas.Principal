namespace McpVentas.Infrastructure.Options;

/// <summary>
/// Clase de configuracion tipada para la seccion "Server" de la configuracion.
/// Controla como se expone el servidor MCP por HTTP.
/// </summary>
public sealed class ServerOptions
{
    /// <summary>Nombre de la seccion de configuracion ("Server").</summary>
    public const string SectionName = "Server";

    /// <summary>URL base donde escucha el servidor MCP (Kestrel).</summary>
    public string Url { get; set; } = "http://localhost:3001";

    /// <summary>
    /// Lista de hosts permitidos por Kestrel (separados por ';').
    /// Protege contra DNS rebinding; en produccion usar los hosts reales.
    /// </summary>
    public string AllowedHosts { get; set; } = "localhost;127.0.0.1";

    /// <summary>
    /// Texto de instrucciones que el servidor MCP expone al cliente/LLM
    /// durante la inicializacion. Ayuda al modelo a saber cuando usar las tools.
    /// </summary>
    public string Instructions { get; set; } = "MCP Server de analitica de ventas del POS (pos_caja).";
}
