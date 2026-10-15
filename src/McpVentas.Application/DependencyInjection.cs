using Microsoft.Extensions.DependencyInjection;
using McpVentas.Application.Services;

namespace McpVentas.Application;

/// <summary>
/// Extension de configuracion de dependencias para la capa Application.
/// Registra los casos de uso para que sean resolubles por el contenedor.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra los servicios de aplicacion en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">Coleccion de servicios de .NET.</param>
    /// <returns>La misma coleccion para permitir encadenamiento.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra los casos de uso de ventas como servicio "scoped".
        services.AddScoped<IVentaService, VentaService>();
        // Registra los casos de uso de articulos como servicio "scoped".
        services.AddScoped<IArticuloService, ArticuloService>();
        // Devuelve la coleccion para encadenar mas registros.
        return services;
    }
}
