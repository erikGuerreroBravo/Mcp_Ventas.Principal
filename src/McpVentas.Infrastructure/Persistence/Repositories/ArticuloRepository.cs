// Importa tipos del sistema.
using System;
// Importa colecciones genericas.
using System.Collections.Generic;
// Importa LINQ.
using System.Linq;
// Importa tipos asincronos.
using System.Threading;
using System.Threading.Tasks;
// Importa el contrato (puerto) y DTOs de Application.
using McpVentas.Application.Abstractions;
using McpVentas.Application.Dtos;
// Importa las entidades mapeadas.
using McpVentas.Infrastructure.Persistence.Entities;
// Importa el contexto de datos.
using McpVentas.Infrastructure.Persistence;
// Importa extensiones de EF Core.
using Microsoft.EntityFrameworkCore;
// Importa la abstraccion de logging.
using Microsoft.Extensions.Logging;

namespace McpVentas.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementacion con EF Core del contrato <see cref="IArticuloRepository"/>.
/// Consultas de solo lectura sobre <c>articulo</c> y <c>venta_articulo</c>.
/// </summary>
public sealed class ArticuloRepository : IArticuloRepository
{
    // Contexto de base de datos.
    private readonly PosCajaDbContext _db;
    // Logger del repositorio.
    private readonly ILogger<ArticuloRepository> _logger;

    /// <summary>Constructor con inyeccion de dependencias.</summary>
    public ArticuloRepository(PosCajaDbContext db, ILogger<ArticuloRepository> logger)
    {
        // Guarda el contexto.
        _db = db;
        // Guarda el logger.
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ArticuloVendidoDto>> TopVendidosAsync(
        DateTime? desde,
        DateTime? hasta,
        int top,
        CancellationToken ct)
    {
        // Punto de partida: renglones vendidos, sin seguimiento de cambios.
        IQueryable<VentaArticulo> query = _db.VentaArticulos.AsNoTracking();

        // Filtra por fecha inicial navegando a la venta del renglon.
        if (desde.HasValue)
        {
            query = query.Where(va => va.Venta.fecha_venta >= desde.Value);
        }
        // Filtra por fecha final navegando a la venta del renglon.
        if (hasta.HasValue)
        {
            query = query.Where(va => va.Venta.fecha_venta <= hasta.Value);
        }

        // Agrupa por codigo de barras y calcula los totales por articulo.
        // NOTA: se agrupa solo por el codigo (clave escalar) porque agrupar por la
        // navegacion Articulo.descripcion impide que EF Core traduzca los SUM a SQL.
        // La descripcion se resuelve con una subconsulta correlacionada en la misma
        // consulta: asi se evita un segundo viaje y el uso de Contains(coleccion),
        // que EF Core traduce a OPENJSON y falla con compatibility_level < 130.
        var agrupado = await query
            .GroupBy(va => va.cod_barras)
            .Select(g => new
            {
                CodBarras = g.Key,
                Cantidad = g.Sum(x => x.cantidad),
                Importe = g.Sum(x => x.precio_vta * x.cantidad),
                Utilidad = g.Sum(x => x.utilidad),
                Descripcion = _db.Articulos
                    .Where(a => a.cod_barras == g.Key)
                    .Select(a => a.descripcion)
                    .FirstOrDefault()
            })
            // Ordena por cantidad vendida descendente y limita al top solicitado.
            .OrderByDescending(x => x.Cantidad)
            .Take(top)
            .ToListAsync(ct);

        // Proyecta el resultado final combinando los totales con la descripcion.
        var resultado = agrupado
            .Select(x => new ArticuloVendidoDto(
                x.CodBarras,
                string.IsNullOrEmpty(x.Descripcion) ? x.CodBarras : x.Descripcion,
                x.Cantidad,
                x.Importe,
                x.Utilidad))
            .ToList();

        // Registra cuantos articulos se devolvieron.
        _logger.LogDebug("TopVendidos devolvio {Count} articulos", resultado.Count);

        // Devuelve la lista.
        return resultado;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ArticuloStockDto>> BajoStockAsync(int limite, CancellationToken ct)
    {
        // Selecciona articulos cuyo stock es menor o igual al minimo configurado.
        var resultado = await _db.Articulos
            .AsNoTracking()
            .Where(a => a.stock <= a.stock_min)
            // Los mas criticos primero (mayor faltante respecto al minimo).
            .OrderBy(a => a.stock - a.stock_min)
            .Take(limite)
            .Select(a => new ArticuloStockDto(
                a.cod_barras,
                a.descripcion,
                a.stock,
                a.stock_min,
                a.stock_max,
                // Faltante calculado hacia el stock maximo.
                a.stock_max - a.stock,
                a.precio_venta))
            .ToListAsync(ct);

        // Registra cuantos articulos bajo stock se detectaron.
        _logger.LogDebug("BajoStock devolvio {Count} articulos", resultado.Count);

        // Devuelve la lista.
        return resultado;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ArticuloDto>> BuscarAsync(
        string? texto,
        string? codBarras,
        int limite,
        CancellationToken ct)
    {
        // Punto de partida: catalogo de articulos sin seguimiento.
        IQueryable<Articulo> query = _db.Articulos.AsNoTracking();

        // Filtra por codigo de barras exacto si se proporciono.
        if (!string.IsNullOrWhiteSpace(codBarras))
        {
            query = query.Where(a => a.cod_barras == codBarras);
        }
        // Filtra por texto en descripciones (LIKE) si se proporciono.
        if (!string.IsNullOrWhiteSpace(texto))
        {
            // Patron de busqueda con comodines.
            var patron = $"%{texto}%";
            query = query.Where(a =>
                EF.Functions.Like(a.descripcion, patron) ||
                EF.Functions.Like(a.descripcion_corta, patron));
        }

        // Ordena por descripcion y limita; proyecta al DTO de articulo.
        var resultado = await query
            .OrderBy(a => a.descripcion)
            .Take(limite)
            .Select(a => new ArticuloDto(
                a.cod_barras,
                a.descripcion,
                a.descripcion_corta,
                a.precio_venta,
                a.precio_compra,
                a.stock,
                a.stock_min,
                a.stock_max,
                a.tipo_articulo,
                a.iva))
            .ToListAsync(ct);

        // Registra cuantos articulos se encontraron.
        _logger.LogDebug("Buscar devolvio {Count} articulos", resultado.Count);

        // Devuelve la lista.
        return resultado;
    }
}
