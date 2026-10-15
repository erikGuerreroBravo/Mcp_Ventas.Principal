// Los DTOs (Data Transfer Objects) son la representacion de salida que se
// serializa a JSON y se devuelve al cliente MCP. Son "records" inmutables:
// ideales para respuestas de solo lectura.
namespace McpVentas.Application.Dtos;

/// <summary>
/// Pagina generica de resultados. Envuelve cualquier listado con metadatos
/// de paginacion para no saturar al consumidor (LLM) con miles de filas.
/// </summary>
/// <typeparam name="T">Tipo de elemento contenido en la pagina.</typeparam>
/// <param name="Elementos">Elementos de la pagina actual.</param>
/// <param name="Total">Numero total de registros que cumplen el filtro.</param>
/// <param name="Pagina">Numero de pagina solicitado (1-based).</param>
/// <param name="Tamanio">Cantidad maxima de elementos por pagina.</param>
public sealed record PaginaDto<T>(
    IReadOnlyList<T> Elementos,
    int Total,
    int Pagina,
    int Tamanio)
{
    /// <summary>
    /// Cantidad total de paginas calculada a partir de <see cref="Total"/>
    /// y <see cref="Tamanio"/>. Si no hay elementos, devuelve 0.
    /// </summary>
    public int TotalPaginas => Tamanio > 0 ? (int)Math.Ceiling(Total / (double)Tamanio) : 0;
}
