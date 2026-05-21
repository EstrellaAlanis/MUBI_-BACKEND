using Microsoft.EntityFrameworkCore;
using Mubi.Api.Models;
using Mubi.Api.Security;

namespace Mubi.Api.Data;

public class MubiDbContext : DbContext
{
    public MubiDbContext(DbContextOptions<MubiDbContext> options) : base(options) {}

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<ComprobanteVenta> ComprobantesVenta => Set<ComprobanteVenta>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<ImagenProducto> ImagenesProducto => Set<ImagenProducto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
    public DbSet<HistorialEstadoPedido> HistorialEstadosPedido => Set<HistorialEstadoPedido>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Material> Materiales => Set<Material>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
    public DbSet<ConsumoMaterial> ConsumosMaterial => Set<ConsumoMaterial>();
    public DbSet<Contacto> Contactos => Set<Contacto>();
    public DbSet<RecuperacionContrasena> RecuperacionesContrasena => Set<RecuperacionContrasena>();
    public DbSet<CodigoVerificacion> CodigosVerificacion => Set<CodigoVerificacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(x => x.IdRol);
            entity.Property(x => x.IdRol).HasColumnName("id_rol");
            entity.Property(x => x.NombreRol).HasColumnName("nombre_rol");
            entity.Property(x => x.Descripcion).HasColumnName("descripcion");
            entity.HasIndex(x => x.NombreRol).IsUnique();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(x => x.IdUsuario);
            entity.Property(x => x.IdUsuario).HasColumnName("id_usuario");
            entity.Property(x => x.Nombre).HasColumnName("nombre");
            entity.Property(x => x.Apellido).HasColumnName("apellido");
            entity.Property(x => x.Correo).HasColumnName("correo");
            entity.Property(x => x.Contrasena).HasColumnName("password_hash");
            entity.Property(x => x.Estado).HasColumnName("estado").HasDefaultValue("activo");
            entity.Property(x => x.FechaRegistro).HasColumnName("fecha_registro");
            entity.Property(x => x.IdRol).HasColumnName("id_rol");
            entity.HasIndex(x => x.Correo).IsUnique();

            entity.HasOne(x => x.Rol).WithMany(x => x.Usuarios).HasForeignKey(x => x.IdRol);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("clientes");
            entity.HasKey(x => x.IdCliente);
            entity.Property(x => x.IdCliente).HasColumnName("id_cliente");
            entity.Property(x => x.Nombres).HasColumnName("nombres");
            entity.Property(x => x.Apellidos).HasColumnName("apellidos");
            entity.Property(x => x.Correo).HasColumnName("correo");
            entity.Property(x => x.Telefono).HasColumnName("telefono");
            entity.Property(x => x.Direccion).HasColumnName("direccion");
            entity.Property(x => x.ReferenciaDireccion).HasColumnName("referencia_direccion");
            entity.Property(x => x.TipoCliente).HasColumnName("tipo_cliente").HasDefaultValue("persona");
            entity.Property(x => x.Ruc).HasColumnName("ruc");
            entity.Property(x => x.RazonSocial).HasColumnName("razon_social");
            entity.Property(x => x.DocumentoIdentidad).HasColumnName("documento_identidad");
            entity.Property(x => x.FechaRegistro).HasColumnName("fecha_registro");
            entity.Property(x => x.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(x => x.Usuario)
                .WithOne(x => x.Cliente)
                .HasForeignKey<Cliente>(x => x.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("categorias");
            entity.HasKey(x => x.IdCategoria);
            entity.Property(x => x.IdCategoria).HasColumnName("id_categoria");
            entity.Property(x => x.NombreCategoria).HasColumnName("nombre_categoria");
            entity.Property(x => x.Descripcion).HasColumnName("descripcion");
            entity.Property(x => x.Estado).HasColumnName("estado");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("productos");
            entity.HasKey(x => x.IdProducto);
            entity.Property(x => x.IdProducto).HasColumnName("id_producto");
            entity.Property(x => x.Nombre).HasColumnName("nombre");
            entity.Property(x => x.Descripcion).HasColumnName("descripcion");
            entity.Property(x => x.Precio).HasColumnName("precio").HasPrecision(10, 2);
            entity.Property(x => x.Disponibilidad).HasColumnName("disponibilidad");
            entity.Property(x => x.FechaRegistro).HasColumnName("fecha_registro");
            entity.Property(x => x.IdCategoria).HasColumnName("id_categoria");

            entity.HasOne(x => x.Categoria)
                .WithMany(x => x.Productos)
                .HasForeignKey(x => x.IdCategoria);
        });

        modelBuilder.Entity<ImagenProducto>(entity =>
        {
            entity.ToTable("producto_imagenes");
            entity.HasKey(x => x.IdImagenProducto);
            entity.Property(x => x.IdImagenProducto).HasColumnName("id_imagen");
            entity.Property(x => x.RutaImagen).HasColumnName("ruta_imagen");
            entity.Property(x => x.Descripcion).HasColumnName("descripcion");
            entity.Property(x => x.EsPrincipal).HasColumnName("es_principal");
            entity.Property(x => x.IdProducto).HasColumnName("id_producto");

            entity.HasOne(x => x.Producto)
                .WithMany(x => x.Imagenes)
                .HasForeignKey(x => x.IdProducto);
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("pedidos", tb => tb.HasTrigger("TR_pedidos"));
            entity.HasKey(x => x.IdPedido);
            entity.Property(x => x.IdPedido).HasColumnName("id_pedido");
            entity.Property(x => x.FechaPedido).HasColumnName("fecha_pedido");
            entity.Property(x => x.EstadoPedido).HasColumnName("estado_pedido");
            entity.Property(x => x.MontoTotal).HasColumnName("monto_total").HasPrecision(10, 2);
            entity.Property(x => x.SaldoPendiente).HasColumnName("saldo_pendiente").HasPrecision(10, 2);
            entity.Property(x => x.Observaciones).HasColumnName("observaciones");
            entity.Property(x => x.RutaExcelTallas).HasColumnName("ruta_excel_tallas");
            entity.Property(x => x.FechaActualizacion).HasColumnName("fecha_actualizacion");
            entity.Property(x => x.IdCliente).HasColumnName("id_cliente");

            entity.HasOne(x => x.Cliente)
                .WithMany(x => x.Pedidos)
                .HasForeignKey(x => x.IdCliente);
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.ToTable("pedido_detalles", tb => tb.HasTrigger("TR_pedido_detalles_subtotal"));
            entity.HasKey(x => x.IdDetallePedido);
            entity.Property(x => x.IdDetallePedido).HasColumnName("id_detalle");
            entity.Property(x => x.Talla).HasColumnName("talla");
            entity.Property(x => x.Color).HasColumnName("color");
            entity.Property(x => x.Cantidad).HasColumnName("cantidad");
            entity.Property(x => x.DisenoPersonalizado).HasColumnName("diseno_personalizado");
            entity.Property(x => x.DescripcionDiseno).HasColumnName("descripcion_diseno");
            entity.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario").HasPrecision(10, 2);
            entity.Property(x => x.Subtotal).HasColumnName("subtotal").HasPrecision(10, 2).ValueGeneratedOnAddOrUpdate();
            entity.Property(x => x.IdPedido).HasColumnName("id_pedido");
            entity.Property(x => x.IdProducto).HasColumnName("id_producto");
            entity.Property(x => x.RutaDisenoFrontal).HasColumnName("ruta_diseno_frontal");
            entity.Property(x => x.RutaDisenoPosterior).HasColumnName("ruta_diseno_posterior");

            entity.HasOne(x => x.Pedido)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => x.IdPedido);

            entity.HasOne(x => x.Producto)
                .WithMany(x => x.DetallesPedido)
                .HasForeignKey(x => x.IdProducto);
        });

        modelBuilder.Entity<HistorialEstadoPedido>(entity =>
        {
            entity.ToTable("historial_estados_pedido", tb => tb.HasTrigger("TR_historial_estados_pedido"));
            entity.HasKey(x => x.IdHistorialEstadoPedido);
            entity.Property(x => x.IdHistorialEstadoPedido).HasColumnName("id_historial");
            entity.Property(x => x.EstadoAnterior).HasColumnName("estado_anterior");
            entity.Property(x => x.EstadoNuevo).HasColumnName("estado_nuevo");
            entity.Property(x => x.Observacion).HasColumnName("comentario");
            entity.Property(x => x.FechaCambio).HasColumnName("fecha_cambio");
            entity.Property(x => x.IdPedido).HasColumnName("id_pedido");

            entity.HasOne(x => x.Pedido)
                .WithMany(x => x.HistorialEstados)
                .HasForeignKey(x => x.IdPedido);
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.ToTable("pagos", tb => tb.HasTrigger("TR_pagos"));
            entity.HasKey(x => x.IdPago);
            entity.Property(x => x.IdPago).HasColumnName("id_pago");
            entity.Property(x => x.Monto).HasColumnName("monto").HasPrecision(10, 2);
            entity.Property(x => x.FechaPago).HasColumnName("fecha_pago");
            entity.Property(x => x.MetodoPago).HasColumnName("metodo_pago");
            entity.Property(x => x.Comprobante).HasColumnName("comprobante");
            entity.Property(x => x.TipoPago).HasColumnName("tipo_pago");
            entity.Property(x => x.IdPedido).HasColumnName("id_pedido");

            entity.HasOne(x => x.Pedido)
                .WithMany(x => x.Pagos)
                .HasForeignKey(x => x.IdPedido);
        });
        modelBuilder.Entity<ComprobanteVenta>(entity =>
        {
            entity.ToTable("comprobantes_venta");
            entity.HasKey(x => x.IdComprobante);

            entity.Property(x => x.IdComprobante).HasColumnName("id_comprobante");
            entity.Property(x => x.TipoComprobante).HasColumnName("tipo_comprobante");
            entity.Property(x => x.Serie).HasColumnName("serie");
            entity.Property(x => x.Numero).HasColumnName("numero");
            entity.Property(x => x.FechaEmision).HasColumnName("fecha_emision");
            entity.Property(x => x.Subtotal).HasColumnName("subtotal").HasPrecision(10, 2);
            entity.Property(x => x.Igv).HasColumnName("igv").HasPrecision(10, 2);
            entity.Property(x => x.Total).HasColumnName("total").HasPrecision(10, 2);
            entity.Property(x => x.Estado).HasColumnName("estado");
            entity.Property(x => x.Observacion).HasColumnName("observacion");
            entity.Property(x => x.IdPedido).HasColumnName("id_pedido");
            entity.Property(x => x.IdCliente).HasColumnName("id_cliente");

            entity.HasIndex(x => new { x.TipoComprobante, x.Serie, x.Numero }).IsUnique();

            entity.HasOne(x => x.Pedido)
                .WithMany()
                .HasForeignKey(x => x.IdPedido);

            entity.HasOne(x => x.Cliente)
                .WithMany()
                .HasForeignKey(x => x.IdCliente);
        });
        
        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("materiales");
            entity.HasKey(x => x.IdMaterial);
            entity.Ignore(x => x.Consumos);
            entity.Property(x => x.IdMaterial).HasColumnName("id_material");
            entity.Property(x => x.NombreMaterial).HasColumnName("nombre_material");
            entity.Property(x => x.Descripcion).HasColumnName("descripcion");
            entity.Property(x => x.StockActual).HasColumnName("stock_actual").HasPrecision(10, 2);
            entity.Property(x => x.StockMinimo).HasColumnName("stock_minimo").HasPrecision(10, 2);
            entity.Property(x => x.UnidadMedida).HasColumnName("unidad_medida");
            entity.Property(x => x.Estado).HasColumnName("estado");
            
            entity.Ignore(x => x.Consumos);
        });

        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            entity.ToTable("movimientos_inventario");
            entity.HasKey(x => x.IdMovimientoInventario);
            entity.Property(x => x.IdMovimientoInventario).HasColumnName("id_movimiento");
            entity.Property(x => x.TipoMovimiento).HasColumnName("tipo_movimiento");
            entity.Property(x => x.Cantidad).HasColumnName("cantidad").HasPrecision(10, 2);
            entity.Property(x => x.Motivo).HasColumnName("motivo");
            entity.Property(x => x.FechaMovimiento).HasColumnName("fecha_movimiento");
            entity.Property(x => x.IdMaterial).HasColumnName("id_material");
        });

        modelBuilder.Entity<ConsumoMaterial>(entity =>
            {
                entity.ToTable("consumo_materiales", tb => tb.UseSqlOutputClause(false));
                entity.HasKey(x => x.IdConsumoMaterial);

                entity.Property(x => x.IdConsumoMaterial).HasColumnName("id_consumo");
                entity.Property(x => x.CantidadUsada).HasColumnName("cantidad_usada").HasPrecision(10, 2);
                entity.Property(x => x.FechaRegistro).HasColumnName("fecha_registro");
                entity.Property(x => x.IdMaterial).HasColumnName("id_material");
                entity.Property(x => x.IdPedido).HasColumnName("id_pedido");

                entity.HasOne(x => x.Pedido)
                    .WithMany(x => x.ConsumosMaterial)
                    .HasForeignKey(x => x.IdPedido);

                entity.Ignore(x => x.Material);
            });

        modelBuilder.Entity<Contacto>(entity =>
        {
            entity.ToTable("contactos");
            entity.HasKey(x => x.IdContacto);
            entity.Property(x => x.IdContacto).HasColumnName("id_contacto");
            entity.Property(x => x.NombreCompleto).HasColumnName("nombre");
            entity.Property(x => x.Correo).HasColumnName("correo");
            entity.Property(x => x.Telefono).HasColumnName("telefono");
            entity.Property(x => x.Asunto).HasColumnName("asunto");
            entity.Property(x => x.Mensaje).HasColumnName("mensaje");
            entity.Property(x => x.FechaRegistro).HasColumnName("fecha_registro");
            entity.Property(x => x.Estado).HasColumnName("estado");
        });

        modelBuilder.Entity<RecuperacionContrasena>(entity =>
        {
            entity.ToTable("recuperacion_password");
            entity.HasKey(x => x.IdRecuperacionContrasena);
            entity.Property(x => x.IdRecuperacionContrasena).HasColumnName("id_recuperacion");
            entity.Property(x => x.Token).HasColumnName("token");
            entity.Property(x => x.FechaSolicitud).HasColumnName("fecha_solicitud");
            entity.Property(x => x.FechaExpiracion).HasColumnName("fecha_expiracion");
            entity.Property(x => x.Usado).HasColumnName("usado");
            
        });
        modelBuilder.Entity<CodigoVerificacion>(entity =>
        {
            entity.ToTable("codigos_verificacion");
            entity.HasKey(x => x.IdCodigo);
            entity.Property(x => x.IdCodigo).HasColumnName("id_codigo");
            entity.Property(x => x.Correo).HasColumnName("correo").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(10).IsRequired();
            entity.Property(x => x.Proposito).HasColumnName("proposito").HasMaxLength(30).IsRequired();
            entity.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(x => x.FechaExpiracion).HasColumnName("fecha_expiracion");
            entity.Property(x => x.Usado).HasColumnName("usado");
            entity.HasIndex(x => new { x.Correo, x.Proposito, x.Usado });
        });
    }
}