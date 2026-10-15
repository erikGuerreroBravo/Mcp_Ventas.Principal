// Importa el serializador JSON de System.Text.Json.
using System.Text.Json;

namespace McpVentas.Server.Tools;

/// <summary>
/// Utilidad compartida por las herramientas MCP para serializar respuestas
/// a JSON con una configuracion uniforme (nombres en camelCase).
/// </summary>
internal static class ToolJson
{
    /// <summary>
    /// Opciones de serializacion: convenciones web (camelCase) y sin indentar
    /// para reducir el tamano de la respuesta que consume el LLM.
    /// </summary>
    private static readonly JsonSerializerOptions Opciones = new(JsonSerializerDefaults.Web)
    {
        // Respuestas compactas (sin espacios ni saltos de linea).
        WriteIndented = false
    };

    /// <summary>Serializa un objeto cualquiera a una cadena JSON.</summary>
    /// <param name="valor">Valor a serializar (DTO, lista, objeto anonimo).</param>
    /// <returns>Cadena JSON lista para devolver como contenido de texto MCP.</returns>
    public static string Serializar(object valor) => JsonSerializer.Serialize(valor, Opciones);
}
