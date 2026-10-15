namespace McpVentas.Application.Dtos;

/// <summary>
/// Ventas agrupadas por vendedor. El nombre completo proviene de <c>empleado</c>
/// (join por <c>user_name</c>), si existe; de lo contrario se usa el usuario crudo.
/// </summary>
/// <param name="Vendedor">Nombre de usuario del vendedor (<c>venta.vendedor</c>).</param>
/// <param name="NombreCompleto">Nombre formateado desde <c>empleado</c> (puede estar vacio).</param>
/// <param name="NumeroVentas">Cantidad de ventas del vendedor.</param>
/// <param name="TotalVendido">Suma de importes vendidos.</param>
/// <param name="TicketPromedio">Importe promedio por venta.</param>
public sealed record VentaPorVendedorDto(
    string Vendedor,
    string NombreCompleto,
    int NumeroVentas,
    decimal TotalVendido,
    decimal TicketPromedio);
