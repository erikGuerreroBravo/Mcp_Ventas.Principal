// Importa tipos base del sistema (Guid, DateTime, etc.).
using System;
// Importa las colecciones genericas.
using System.Collections.Generic;
// Importa las entidades scaffolded.
using McpVentas.Infrastructure.Persistence.Entities;
// Importa EF Core (DbContext, ModelBuilder, etc.).
using Microsoft.EntityFrameworkCore;

namespace McpVentas.Infrastructure.Persistence;

/// <summary>
/// Contexto de EF Core para la base de datos <c>pos_caja</c>.
/// Fue generado por ingenieria inversa (Database-First) para las tablas
/// <c>venta</c>, <c>venta_articulo</c>, <c>articulo</c> y <c>empleado</c>.
/// La cadena de conexion NO se define aqui (se genero con --no-onconfiguring);
/// se configura desde la capa de composicion mediante inyeccion de dependencias.
/// Es <c>partial</c> para permitir extensiones sin tocar el codigo generado.
/// </summary>
public partial class PosCajaDbContext : DbContext
{
    /// <summary>
    /// Constructor que recibe las opciones (incluida la cadena de conexion y el provider)
    /// configuradas en el contenedor de dependencias.
    /// </summary>
    /// <param name="options">Opciones de EF Core del contexto.</param>
    public PosCajaDbContext(DbContextOptions<PosCajaDbContext> options)
        : base(options)
    {
    }

    /// <summary>Conjunto de articulos del catalogo.</summary>
    public virtual DbSet<Articulo> Articulos { get; set; }

    /// <summary>Conjunto de empleados/vendedores.</summary>
    public virtual DbSet<Empleado> Empleados { get; set; }

    /// <summary>Conjunto de renglones (lineas) de venta.</summary>
    public virtual DbSet<VentaArticulo> VentaArticulos { get; set; }

    /// <summary>Conjunto de ventas (cabeceras de ticket).</summary>
    public virtual DbSet<Venta> Ventas { get; set; }

    /// <summary>
    /// Configura el modelo mapeando entidades y propiedades a las tablas y columnas
    /// reales de la base de datos. Todo esto lo genero EF Core a partir del esquema.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fija la collation del modelo a la usada por la base de datos.
        modelBuilder.UseCollation("Modern_Spanish_CI_AS");

        // ---- Mapeo de la tabla 'articulo' ----
        modelBuilder.Entity<Articulo>(entity =>
        {
            // Clave primaria: cod_barras.
            entity.HasKey(e => e.cod_barras).HasName("pk_articulo");
            // Nombre fisico de la tabla.
            entity.ToTable("articulo");
            // Indice unico compuesto (cod_barras, cod_asociado).
            entity.HasIndex(e => new { e.cod_barras, e.cod_asociado }, "ix_articulo").IsUnique();
            // Indices secundarios.
            entity.HasIndex(e => e.cod_asociado, "ix_relationship1");
            entity.HasIndex(e => e.id_clasificacion, "ix_relationship2");
            entity.HasIndex(e => e.id_unidad, "ix_relationship3");
            entity.HasIndex(e => e.id_proveedor, "ix_relationship4");
            // Longitudes y tipos de columnas.
            entity.Property(e => e.cod_barras)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.articulo_disponible).HasDefaultValue(true);
            entity.Property(e => e.cantidad_um)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(19, 3)");
            entity.Property(e => e.cod_asociado)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.cod_interno)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.cve_producto)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("0");
            entity.Property(e => e.descripcion)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.descripcion_corta)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.fecha_registro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.id_objeto_impuesto)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.iva)
                .HasDefaultValue(0.16m)
                .HasColumnType("decimal(4, 2)");
            entity.Property(e => e.kit_fecha_fin).HasColumnType("datetime");
            entity.Property(e => e.kit_fecha_ini).HasColumnType("datetime");
            entity.Property(e => e.last_update_inventory)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.precio_compra).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.precio_venta).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.stock).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.stock_max).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.stock_min).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.tipo_articulo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("principal");
            entity.Property(e => e.utilidad).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.visible).HasDefaultValue(true);
        });

        // ---- Mapeo de la tabla 'empleado' ----
        modelBuilder.Entity<Empleado>(entity =>
        {
            // Clave primaria: id_empleado.
            entity.HasKey(e => e.id_empleado).HasName("pk_empleado");
            entity.ToTable("empleado");
            // Indice por nombre de usuario (se cruza con venta.vendedor).
            entity.HasIndex(e => e.user_name, "ix_relationship10");
            // El GUID lo genera la aplicacion (no la base de datos).
            entity.Property(e => e.id_empleado).ValueGeneratedNever();
            entity.Property(e => e.a_materno)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.a_paterno)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.fecha_registro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.nombre)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.user_name)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        // ---- Mapeo de la tabla 'venta_articulo' ----
        modelBuilder.Entity<VentaArticulo>(entity =>
        {
            // Clave primaria compuesta: (no_articulo, id_pos, id_venta).
            entity.HasKey(e => new { e.no_articulo, e.id_pos, e.id_venta }).HasName("pk_venta_articulo");
            entity.ToTable("venta_articulo");
            // Indices secundarios.
            entity.HasIndex(e => new { e.id_venta, e.id_pos }, "ix_relationship1");
            entity.HasIndex(e => e.cod_barras, "ix_relationship2");
            entity.HasIndex(e => e.user_name, "ix_relationship3");
            entity.HasIndex(e => e.id_promo, "ix_relationship4");
            // Tipos de columna decimal.
            entity.Property(e => e.cant_devuelta).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.cantidad).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.cod_barras)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ieps).HasColumnType("decimal(4, 3)");
            entity.Property(e => e.iva).HasColumnType("decimal(4, 3)");
            entity.Property(e => e.porcent_desc).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.precio_compra).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.precio_regular).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.precio_vta).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.user_name)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.utilidad).HasColumnType("decimal(19, 3)");

            // Relacion N a 1 con 'articulo' por cod_barras.
            entity.HasOne(d => d.Articulo).WithMany(p => p.VentaArticulos)
                .HasForeignKey(d => d.cod_barras)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_articulo_venta");

            // Relacion N a 1 con 'venta' por (id_venta, id_pos).
            entity.HasOne(d => d.Venta).WithMany(p => p.Lineas)
                .HasForeignKey(d => new { d.id_venta, d.id_pos })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_venta_articulo");
        });

        // ---- Mapeo de la tabla 'venta' ----
        modelBuilder.Entity<Venta>(entity =>
        {
            // Clave primaria compuesta: (id_venta, id_pos).
            entity.HasKey(e => new { e.id_venta, e.id_pos }).HasName("pk_venta");
            // Nombre fisico de la tabla (en singular y en minusculas).
            entity.ToTable("venta");
            // Indices secundarios.
            entity.HasIndex(e => e.id_pos, "ix_relationship2");
            entity.HasIndex(e => e.supervisor, "ix_relationship3");
            entity.HasIndex(e => e.vendedor, "ix_relationship4");
            // Tipos de columna (nota: varias columnas de pago usan escala 2 y el resto 3).
            entity.Property(e => e.descuento).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.fecha_venta).HasColumnType("datetime");
            entity.Property(e => e.ieps_desglosado).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.impuestos).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.iva_desglosado).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.pago_cheque).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.pago_efectivo).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.pago_spei).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.pago_tc).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.pago_td).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.pago_vales).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.subtotal).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.supervisor)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.total_vendido).HasColumnType("decimal(19, 3)");
            entity.Property(e => e.vendedor)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        // Permite ampliar el modelo desde un archivo parcial del usuario.
        OnModelCreatingPartial(modelBuilder);
    }

    /// <summary>Punto de extension parcial para agregar configuracion adicional del modelo.</summary>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
