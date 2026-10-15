// Importa las abstracciones de configuracion (IConfiguration).
using Microsoft.Extensions.Configuration;
// Importa las abstracciones del contenedor de dependencias (IServiceCollection).
using Microsoft.Extensions.DependencyInjection;
// Importa EF Core (UseSqlServer, QueryTrackingBehavior).
using Microsoft.EntityFrameworkCore;
// Importa la capa Application para registrar sus casos de uso.
using McpVentas.Application;
// Importa los contratos (puertos) definidos en Application.
using McpVentas.Application.Abstractions;
// Importa el contexto y las entidades de persistencia.
using McpVentas.Infrastructure.Persistence;
using McpVentas.Infrastructure.Persistence.Repositories;
// Importa las clases de opciones tipadas.
using McpVentas.Infrastructure.Options;

namespace McpVentas.Infrastructure;

/// <summary>
/// Punto de composicion de la capa Infrastructure. Registra en el contenedor
/// de dependencias el contexto de EF Core, las opciones tipadas y los repositorios.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra todos los servicios de infraestructura y encadena el registro
    /// de la capa Application (AddApplication).
    /// </summary>
    /// <param name="services">Coleccion de servicios de .NET.</param>
    /// <param name="configuration">Configuracion de la aplicacion (appsettings + entorno).</param>
    /// <returns>La misma coleccion para permitir encadenamiento.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Enlaza la seccion "Database" a la clase DatabaseOptions.
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        // Enlaza la seccion "Server" a la clase ServerOptions.
        services.Configure<ServerOptions>(configuration.GetSection(ServerOptions.SectionName));

        // Materializa las opciones de base de datos para usarlas aqui mismo.
        var dbOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        // Valida que la cadena de conexion este presente (falla rapido si falta).
        if (string.IsNullOrWhiteSpace(dbOptions.ConnectionString))
        {
            // Lanza una excepcion clara que indique como configurarla.
            throw new InvalidOperationException(
                "Falta la cadena de conexion. Configure 'Database:ConnectionString' " +
                "o la variable de entorno 'MCP_Database__ConnectionString'.");
        }

        // Registra el DbContext de EF Core con el proveedor de SQL Server.
        services.AddDbContext<PosCajaDbContext>(options =>
        {
            // Usa la cadena de conexion y habilita reintentos automaticos transitorios.
            options.UseSqlServer(dbOptions.ConnectionString, sql =>
                sql.EnableRetryOnFailure());
            // Por defecto, las consultas de solo lectura no rastrean cambios (mas rapido).
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            // Habilita el log detallado solo si se configuro explicitamente.
            if (dbOptions.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Registra los repositorios que implementan los puertos de Application.
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IArticuloRepository, ArticuloRepository>();

        // Registra los casos de uso de la capa Application.
        services.AddApplication();

        // Devuelve la coleccion para encadenar registros adicionales.
        return services;
    }
}
