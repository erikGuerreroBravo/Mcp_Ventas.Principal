// Importa tipos del sistema (DateTime, Guid, Math).
using System;
// Importa tipos de colecciones genericas.
using System.Collections.Generic;
// Importa LINQ para componer consultas.
using System.Linq;
// Importa tipos de tareas asincronas.
using System.Threading;
using System.Threading.Tasks;
// Importa las abstracciones (contrato) y DTOs de la capa Application.
using McpVentas.Application.Abstractions;
using McpVentas.Application.Dtos;
// Importa las entidades mapeadas por EF Core.
using McpVentas.Infrastructure.Persistence.Entities;
// Importa el contexto de base de datos.
using McpVentas.Infrastructure.Persistence;
// Importa las extensiones de EF Core (ToListAsync, AsNoTracking, EF.Functions, etc.).
using Microsoft.EntityFrameworkCore;
// Importa la abstraccion de logging.
using Microsoft.Extensions.Logging;

namespace McpVentas.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementacion con EF Core del contrato <see cref="IVentaRepository"/>.
/// Traduce los casos de uso a consultas LINQ de solo lectura (AsNoTracking)
/// y proyecta directamente a DTOs para evitar materializar entidades completas.
/// </summary>
public sealed class VentaRepository : IVentaRepository
{
    // Contexto de base de datos inyectado (una instancia por solicitud).
    private readonly PosCajaDbContext _db;
    // Logger del repositorio para trazar consultas y errores.
    private readonly ILogger<VentaRepository> _logger;

    /// <summary>Constructor con inyeccion de dependencias.</summary>
    public VentaRepository(PosCajaDbContext db, ILogger<VentaRepository> logger)
    {
        // Guarda el contexto.
        _db = db;
        // Guarda el logger.
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<PaginaDto<VentaDto>> ListarAsync(
        DateTime? desde,
        DateTime? hasta,
        string? vendedor,
        long? folio,
        int pagina,
        int tamanio,
        CancellationToken ct)
    {
        // Punto de partida: la tabla venta sin seguimiento de cambios (mas rapido).
        IQueryable<Venta> query = _db.Ventas.AsNoTracking();

        // Aplica el filtro de fecha inicial si se proporciono.
        if (desde.HasValue)
        {
            query = query.Where(v => v.fecha_venta >= desde.Value);
        }
        // Aplica el filtro de fecha final si se proporciono.
        if (hasta.HasValue)
        {
            query = query.Where(v => v.fecha_venta <= hasta.Value);
        }
        // Aplica el filtro de vendedor exacto si se proporciono.
        if (!string.IsNullOrWhiteSpace(vendedor))
        {
            query = query.Where(v => v.vendedor == vendedor);
        }
        // Aplica el filtro de folio exacto si se proporciono.
        if (folio.HasValue)
        {
            query = query.Where(v => v.folio == folio.Value);
        }

        // Cuenta el total de registros que cumplen el filtro (para la paginacion).
        var total = await query.CountAsync(ct);

        // Trae solo la pagina solicitada, ordenada por fecha descendente.
        var elementos = await query
            .OrderByDescending(v => v.fecha_venta)
            .ThenByDescending(v => v.folio)
            .Skip((pagina - 1) * tamanio)
            .Take(tamanio)
            .Select(v => new VentaDto(
                v.id_venta,
                v.id_pos,
                v.folio,
                v.fecha_venta,
                v.vendedor,
                v.total_vendido,
                v.subtotal,
                v.impuestos,
                v.descuento,
                v.num_registros))
            .ToListAsync(ct);

        // Registra el resultado de la consulta.
        _logger.LogDebug("ListarAsync devolvio {Count} de {Total} ventas", elementos.Count, total);

        // Empaqueta el resultado con metadatos de paginacion.
        return new PaginaDto<VentaDto>(elementos, total, pagina, tamanio);
    }

    /// <inheritdoc />
    public async Task<VentaDto?> ObtenerPorFolioAsync(long folio, CancellationToken ct)
    {
        // Busca la primera venta con ese folio y proyecta a DTO.
        return await _db.Ventas
            .AsNoTracking()
            .Where(v => v.folio == folio)
            .OrderByDescending(v => v.fecha_venta)
            .Select(v => new VentaDto(
                v.id_venta,
                v.id_pos,
                v.folio,
                v.fecha_venta,
                v.vendedor,
                v.total_vendido,
                v.subtotal,
                v.impuestos,
                v.descuento,
                v.num_registros))
            // Devuelve null si no existe (en lugar de lanzar excepcion).
            .FirstOrDefaultAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<VentaDetalleDto>> ObtenerDetalleAsync(
        long? folio,
        Guid? idVenta,
        CancellationToken ct)
    {
        // Punto de partida: renglones de venta sin seguimiento.
        IQueryable<VentaArticulo> query = _db.VentaArticulos.AsNoTracking();

        // Filtra por folio si se proporciono (navegando a la venta).
        if (folio.HasValue)
        {
            query = query.Where(va => va.Venta.folio == folio.Value);
        }
        // Filtra por id_venta si se proporciono.
        if (idVenta.HasValue)
        {
            query = query.Where(va => va.id_venta == idVenta.Value);
        }

        // Ordena por numero de renglon y proyecta al DTO de detalle.
        return await query
            .OrderBy(va => va.no_articulo)
            .Select(va => new VentaDetalleDto(
                va.cod_barras,
                va.Articulo.descripcion,
                va.cantidad,
                va.precio_vta,
                // Importe de la linea: cantidad por precio menos el descuento porcentual.
                va.cantidad * va.precio_vta * (1 - va.porcent_desc / 100m),
                va.iva,
                va.porcent_desc,
                va.cant_devuelta))
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<ResumenVentasDto> ResumenPeriodoAsync(
        DateTime? desde,
        DateTime? hasta,
        CancellationToken ct)
    {
        // Aplica los filtros de fecha sobre la tabla venta.
        var query = AplicarFiltroFechas(_db.Ventas.AsNoTracking(), desde, hasta);

        // Agrega todas las filas en una sola consulta (GroupBy con clave constante
        // se traduce a un SELECT de agregados sin GROUP BY).
        var agg = await query
            .GroupBy(v => 1)
            .Select(g => new
            {
                // Cantidad de ventas del periodo.
                NumeroVentas = g.Count(),
                // Sumas monetarias del periodo.
                TotalVendido = g.Sum(x => x.total_vendido),
                Subtotal = g.Sum(x => x.subtotal),
                Impuestos = g.Sum(x => x.impuestos),
                Descuento = g.Sum(x => x.descuento)
            })
            .FirstOrDefaultAsync(ct);

        // Si no hay datos, devuelve un resumen en ceros con el rango solicitado.
        var numeroVentas = agg?.NumeroVentas ?? 0;
        var totalVendido = agg?.TotalVendido ?? 0m;
        // Calcula el ticket promedio evitando division entre cero.
        var ticketPromedio = numeroVentas > 0 ? decimal.Round(totalVendido / numeroVentas, 2) : 0m;

        // Registra el resumen calculado.
        _logger.LogDebug("ResumenPeriodo ventas={NumeroVentas} total={TotalVendido}", numeroVentas, totalVendido);

        // Devuelve el DTO con el rango efectivo (MinValue/MaxValue si venian nulos).
        return new ResumenVentasDto(
            desde ?? DateTime.MinValue,
            hasta ?? DateTime.MaxValue,
            numeroVentas,
            totalVendido,
            agg?.Subtotal ?? 0m,
            agg?.Impuestos ?? 0m,
            agg?.Descuento ?? 0m,
            ticketPromedio);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<VentaPorVendedorDto>> PorVendedorAsync(
        DateTime? desde,
        DateTime? hasta,
        int limite,
        CancellationToken ct)
    {
        // Filtra por fechas y agrupa por vendedor.
        var agrupado = await AplicarFiltroFechas(_db.Ventas.AsNoTracking(), desde, hasta)
            .GroupBy(v => v.vendedor)
            .Select(g => new
            {
                // Clave de agrupacion: el usuario vendedor.
                Vendedor = g.Key,
                // Metricas por vendedor.
                NumeroVentas = g.Count(),
                TotalVendido = g.Sum(x => x.total_vendido)
            })
            // Ordena por importe vendido descendente y limita el resultado.
            .OrderByDescending(x => x.TotalVendido)
            .Take(limite)
            .ToListAsync(ct);

        // Carga el catalogo de empleados para enriquecer con el nombre completo.
        var empleados = await _db.Empleados
            .AsNoTracking()
            .Select(e => new { e.user_name, Nombre = e.nombre, e.a_paterno, e.a_materno })
            .ToListAsync(ct);
        // Indexa por user_name (evita multiples busquedas lineales).
        var mapaEmpleados = empleados
            .GroupBy(e => e.user_name)
            .ToDictionary(g => g.Key, g => g.First());

        // Construye el resultado final combinando el agregado con el nombre.
        var resultado = agrupado
            .Select(x =>
            {
                // Busca el nombre completo del empleado si existe.
                mapaEmpleados.TryGetValue(x.Vendedor, out var emp);
                // Arma el nombre completo; si no hay empleado, deja cadena vacia.
                var nombreCompleto = emp is null
                    ? string.Empty
                    : $"{emp.Nombre} {emp.a_paterno} {emp.a_materno}".Trim();
                // Ticket promedio por vendedor.
                var ticket = x.NumeroVentas > 0 ? decimal.Round(x.TotalVendido / x.NumeroVentas, 2) : 0m;
                // Devuelve el DTO del vendedor.
                return new VentaPorVendedorDto(x.Vendedor, nombreCompleto, x.NumeroVentas, x.TotalVendido, ticket);
            })
            .ToList();

        // Devuelve la lista como solo lectura.
        return resultado;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<VentaPorDiaDto>> PorDiaAsync(
        DateTime? desde,
        DateTime? hasta,
        int limite,
        CancellationToken ct)
    {
        // Filtra por fechas y agrupa por dia.
        // NOTA: se agrupa por Year/Month/Day (se traducen a DATEPART) porque EF Core
        // no traduce DateTime.Date como clave de GroupBy a SQL Server.
        var agrupado = await AplicarFiltroFechas(_db.Ventas.AsNoTracking(), desde, hasta)
            .GroupBy(v => new { v.fecha_venta.Year, v.fecha_venta.Month, v.fecha_venta.Day })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.Day,
                NumeroVentas = g.Count(),
                TotalVendido = g.Sum(x => x.total_vendido)
            })
            // Toma los dias mas recientes segun el limite (orden por componentes).
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ThenByDescending(x => x.Day)
            .Take(limite)
            .ToListAsync(ct);

        // Reconstruye la fecha y ordena de forma ascendente (serie cronologica).
        return agrupado
            .Select(x => new VentaPorDiaDto(
                new DateTime(x.Year, x.Month, x.Day),
                x.NumeroVentas,
                x.TotalVendido))
            .OrderBy(x => x.Fecha)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<VentaPorFormaPagoDto>> PorFormaPagoAsync(
        DateTime? desde,
        DateTime? hasta,
        CancellationToken ct)
    {
        // Agrega en una sola consulta la suma de cada columna de pago.
        var agg = await AplicarFiltroFechas(_db.Ventas.AsNoTracking(), desde, hasta)
            .GroupBy(v => 1)
            .Select(g => new
            {
                Efectivo = g.Sum(x => x.pago_efectivo),
                Cheque = g.Sum(x => x.pago_cheque),
                Vales = g.Sum(x => x.pago_vales),
                TarjetaCredito = g.Sum(x => x.pago_tc),
                TarjetaDebito = g.Sum(x => x.pago_td),
                Spei = g.Sum(x => x.pago_spei)
            })
            .FirstOrDefaultAsync(ct);

        // Total cobrado = suma de todos los medios de pago (para porcentajes).
        var total = (agg?.Efectivo ?? 0m) + (agg?.Cheque ?? 0m) + (agg?.Vales ?? 0m)
                  + (agg?.TarjetaCredito ?? 0m) + (agg?.TarjetaDebito ?? 0m) + (agg?.Spei ?? 0m);

        // Funcion local que calcula el porcentaje respecto al total.
        static decimal Porcentaje(decimal importe, decimal total)
            => total > 0 ? decimal.Round(importe / total * 100m, 2) : 0m;

        // Construye la lista de desglose por forma de pago.
        var lista = new List<VentaPorFormaPagoDto>
        {
            new("Efectivo", "pago_efectivo", agg?.Efectivo ?? 0m, Porcentaje(agg?.Efectivo ?? 0m, total)),
            new("Cheque", "pago_cheque", agg?.Cheque ?? 0m, Porcentaje(agg?.Cheque ?? 0m, total)),
            new("Vales", "pago_vales", agg?.Vales ?? 0m, Porcentaje(agg?.Vales ?? 0m, total)),
            new("Tarjeta de credito", "pago_tc", agg?.TarjetaCredito ?? 0m, Porcentaje(agg?.TarjetaCredito ?? 0m, total)),
            new("Tarjeta de debito", "pago_td", agg?.TarjetaDebito ?? 0m, Porcentaje(agg?.TarjetaDebito ?? 0m, total)),
            new("SPEI", "pago_spei", agg?.Spei ?? 0m, Porcentaje(agg?.Spei ?? 0m, total))
        };

        // Devuelve solo las formas de pago con importe distinto de cero.
        return lista.Where(x => x.Importe != 0m).ToList();
    }

    /// <summary>
    /// Aplica los filtros de fecha (desde/hasta) a una consulta de ventas.
    /// </summary>
    private static IQueryable<Venta> AplicarFiltroFechas(IQueryable<Venta> query, DateTime? desde, DateTime? hasta)
    {
        // Filtro de fecha inicial.
        if (desde.HasValue)
        {
            query = query.Where(v => v.fecha_venta >= desde.Value);
        }
        // Filtro de fecha final.
        if (hasta.HasValue)
        {
            query = query.Where(v => v.fecha_venta <= hasta.Value);
        }
        // Devuelve la consulta (posiblemente filtrada).
        return query;
    }
}
