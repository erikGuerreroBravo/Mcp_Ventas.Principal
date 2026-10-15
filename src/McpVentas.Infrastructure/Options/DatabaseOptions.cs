// Importa el componente base de las clases de opciones.
namespace McpVentas.Infrastructure.Options;

/// <summary>
/// Clase de configuracion tipada para la seccion "Database" de la configuracion.
/// Se enlaza automaticamente desde appsettings.json o variables de entorno
/// (por ejemplo <c>MCP_Database__ConnectionString</c>).
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>
    /// Nombre de la seccion de configuracion a la que se enlaza esta clase.
    /// El binder usa este valor para mapear "Database:ConnectionString".
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    /// Cadena de conexion a la base de datos pos_caja.
    /// Debe proporcionarse por variable de entorno o User Secrets; nunca en codigo.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Indica si EF Core debe habilitar el registro (log) detallado de SQL.
    /// Util en desarrollo; desactivado por defecto para no exponer datos.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; }
}
