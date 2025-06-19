using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SistemaRestaurante.Models;

public partial class SoftwareRestauranteContext : DbContext
{
    public SoftwareRestauranteContext()
    {
    }

    public SoftwareRestauranteContext(DbContextOptions<SoftwareRestauranteContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Mesa> Mesas { get; set; }

    public virtual DbSet<Orden> Ordens { get; set; }

    public virtual DbSet<OrdenPlatillo> OrdenPlatillos { get; set; }

    public virtual DbSet<Platillo> Platillos { get; set; }

    public virtual DbSet<PlatilloProducto> PlatilloProductos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Ventum> Venta { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasKey(e => e.IdMesa).HasName("PK__Mesa__4D7E81B14E04CF22");

            entity.ToTable("Mesa");
        });

        modelBuilder.Entity<Orden>(entity =>
        {
            entity.HasKey(e => e.IdOrden).HasName("PK__Orden__C38F300DAB0FFEF5");

            entity.ToTable("Orden");

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Orden__IdUsuario__6477ECF3");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.MesaId)
                .HasConstraintName("FK__Orden__MesaId__619B8048");
        });

        modelBuilder.Entity<OrdenPlatillo>(entity =>
        {
            entity.HasKey(e => e.IdOrdenPlatillo).HasName("PK__OrdenPla__9A19BA2724940B52");

            entity.ToTable("OrdenPlatillo");

            entity.HasOne(d => d.Orden).WithMany(p => p.OrdenPlatillos)
                .HasForeignKey(d => d.OrdenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdenPlat__Orden__6754599E");

            entity.HasOne(d => d.Platillo).WithMany(p => p.OrdenPlatillos)
                .HasForeignKey(d => d.PlatilloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdenPlat__Plati__68487DD7");
        });

        modelBuilder.Entity<Platillo>(entity =>
        {
            entity.HasKey(e => e.IdPlatillo).HasName("PK__Platillo__C761A718DFA55577");

            entity.ToTable("Platillo");

            entity.Property(e => e.Area).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<PlatilloProducto>(entity =>
        {
            entity.HasKey(e => e.IdPlatilloProducto).HasName("PK__Platillo__DEEDD027E362B796");

            entity.ToTable("PlatilloProducto");

            entity.HasOne(d => d.Platillo).WithMany(p => p.PlatilloProductos)
                .HasForeignKey(d => d.PlatilloId)
                .HasConstraintName("FK__PlatilloP__Plati__5DCAEF64");

            entity.HasOne(d => d.Producto).WithMany(p => p.PlatilloProductos)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__PlatilloP__Produ__5EBF139D");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__0988921003778E17");

            entity.ToTable("Producto");

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__2A49584C49372850");

            entity.ToTable("Rol");

            entity.Property(e => e.Nombre).HasMaxLength(20);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.Property(e => e.IdUsuario).ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Rol");
        });

        modelBuilder.Entity<Ventum>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__Venta__BC1240BD9A68C51F");

            entity.Property(e => e.FechaVenta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PropinaBebidas).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PropinaCocina).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PropinaMesero).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdOrden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Venta_Orden");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
