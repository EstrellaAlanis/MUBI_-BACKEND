
IF DB_ID('MUBI_DB_FINAL') IS NOT NULL
BEGIN
    ALTER DATABASE MUBI_DB_FINAL SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MUBI_DB_FINAL;
END;
GO

CREATE DATABASE MUBI_DB_FINAL;
GO

USE MUBI_DB_FINAL;
GO

/* ==========================
   1. SEGURIDAD Y USUARIOS
   ========================== */
CREATE TABLE roles (
    id_rol INT IDENTITY(1,1) PRIMARY KEY,
    nombre_rol VARCHAR(50) NOT NULL UNIQUE,
    descripcion VARCHAR(150) NULL
);
GO

CREATE TABLE usuarios (
    id_usuario INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    correo VARCHAR(150) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'activo',
    fecha_registro DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    id_rol INT NOT NULL,
    CONSTRAINT fk_usuarios_roles FOREIGN KEY (id_rol) REFERENCES roles(id_rol),
    CONSTRAINT ck_usuarios_estado CHECK (estado IN ('activo','inactivo','bloqueado'))
);
GO

CREATE TABLE recuperacion_password (
    id_recuperacion INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario INT NOT NULL,
    token VARCHAR(255) NOT NULL UNIQUE,
    fecha_solicitud DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    fecha_expiracion DATETIME2 NOT NULL,
    usado BIT NOT NULL DEFAULT 0,
    CONSTRAINT fk_recuperacion_password_usuario FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);
GO

CREATE TABLE clientes (
    id_cliente INT IDENTITY(1,1) PRIMARY KEY,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    correo VARCHAR(150) NOT NULL,
    telefono VARCHAR(20) NULL,
    direccion VARCHAR(200) NULL,
    documento_identidad VARCHAR(20) NULL,
    fecha_registro DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    id_usuario INT NULL,
    CONSTRAINT fk_clientes_usuarios FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);
GO

/* ==========================
   2. PRODUCTOS Y CATÁLOGO
   ========================== */
CREATE TABLE categorias (
    id_categoria INT IDENTITY(1,1) PRIMARY KEY,
    nombre_categoria VARCHAR(100) NOT NULL UNIQUE,
    descripcion VARCHAR(200) NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'activo',
    CONSTRAINT ck_categorias_estado CHECK (estado IN ('activo','inactivo'))
);
GO

CREATE TABLE productos (
    id_producto INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(120) NOT NULL,
    descripcion VARCHAR(300) NULL,
    precio DECIMAL(10,2) NOT NULL,
    disponibilidad VARCHAR(20) NOT NULL DEFAULT 'disponible',
    permite_personalizacion BIT NOT NULL DEFAULT 1,
    fecha_registro DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    id_categoria INT NOT NULL,
    CONSTRAINT fk_productos_categorias FOREIGN KEY (id_categoria) REFERENCES categorias(id_categoria),
    CONSTRAINT ck_productos_precio CHECK (precio > 0),
    CONSTRAINT ck_productos_disponibilidad CHECK (disponibilidad IN ('disponible','agotado','inactivo'))
);
GO

CREATE TABLE producto_imagenes (
    id_imagen INT IDENTITY(1,1) PRIMARY KEY,
    id_producto INT NOT NULL,
    ruta_imagen VARCHAR(255) NOT NULL,
    nombre_archivo VARCHAR(150) NULL,
    descripcion VARCHAR(200) NULL,
    es_principal BIT NOT NULL DEFAULT 0,
    fecha_registro DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_producto_imagenes_producto FOREIGN KEY (id_producto) REFERENCES productos(id_producto)
);
GO

/* ==========================
   3. PEDIDOS
   ========================== */
CREATE TABLE pedidos (
    id_pedido INT IDENTITY(1,1) PRIMARY KEY,
    id_cliente INT NOT NULL,
    fecha_pedido DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    fecha_actualizacion DATETIME2 NULL,
    estado_pedido VARCHAR(20) NOT NULL DEFAULT 'pendiente',
    monto_total DECIMAL(10,2) NOT NULL DEFAULT 0,
    saldo_pendiente DECIMAL(10,2) NOT NULL DEFAULT 0,
    observaciones VARCHAR(300) NULL,
    direccion_entrega VARCHAR(200) NULL,
    requiere_delivery BIT NOT NULL DEFAULT 0,
    CONSTRAINT fk_pedidos_clientes FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente),
    CONSTRAINT ck_pedidos_estado CHECK (estado_pedido IN ('pendiente','confirmado','en_proceso','entregado','cancelado')),
    CONSTRAINT ck_pedidos_total CHECK (monto_total >= 0),
    CONSTRAINT ck_pedidos_saldo CHECK (saldo_pendiente >= 0)
);
GO

CREATE TABLE pedido_detalles (
    id_detalle INT IDENTITY(1,1) PRIMARY KEY,
    id_pedido INT NOT NULL,
    id_producto INT NOT NULL,
    talla VARCHAR(10) NOT NULL,
    color VARCHAR(50) NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    diseno_personalizado VARCHAR(255) NULL,
    archivo_diseno VARCHAR(255) NULL,
    descripcion_diseno VARCHAR(300) NULL,
    subtotal AS (cantidad * precio_unitario) PERSISTED,
    CONSTRAINT fk_pedido_detalles_pedido FOREIGN KEY (id_pedido) REFERENCES pedidos(id_pedido),
    CONSTRAINT fk_pedido_detalles_producto FOREIGN KEY (id_producto) REFERENCES productos(id_producto),
    CONSTRAINT ck_pedido_detalles_cantidad CHECK (cantidad > 0),
    CONSTRAINT ck_pedido_detalles_precio CHECK (precio_unitario > 0),
    CONSTRAINT ck_pedido_detalles_talla CHECK (talla IN ('XS','S','M','L','XL','XXL','XXXL'))
);
GO

CREATE TABLE historial_estados_pedido (
    id_historial INT IDENTITY(1,1) PRIMARY KEY,
    id_pedido INT NOT NULL,
    estado_anterior VARCHAR(20) NULL,
    estado_nuevo VARCHAR(20) NOT NULL,
    comentario VARCHAR(250) NULL,
    fecha_cambio DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    id_usuario INT NULL,
    CONSTRAINT fk_historial_pedido FOREIGN KEY (id_pedido) REFERENCES pedidos(id_pedido),
    CONSTRAINT fk_historial_usuario FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);
GO

/* ==========================
   4. PAGOS
   ========================== */
CREATE TABLE pagos (
    id_pago INT IDENTITY(1,1) PRIMARY KEY,
    id_pedido INT NOT NULL,
    monto DECIMAL(10,2) NOT NULL,
    fecha_pago DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    metodo_pago VARCHAR(30) NOT NULL,
    comprobante VARCHAR(255) NULL,
    tipo_pago VARCHAR(20) NOT NULL,
    estado_pago VARCHAR(20) NOT NULL DEFAULT 'registrado',
    observacion VARCHAR(250) NULL,
    CONSTRAINT fk_pagos_pedido FOREIGN KEY (id_pedido) REFERENCES pedidos(id_pedido),
    CONSTRAINT ck_pagos_monto CHECK (monto > 0),
    CONSTRAINT ck_pagos_metodo CHECK (metodo_pago IN ('Yape','Plin','transferencia','efectivo')),
    CONSTRAINT ck_pagos_tipo CHECK (tipo_pago IN ('adelanto','pago_final','pago_parcial')),
    CONSTRAINT ck_pagos_estado CHECK (estado_pago IN ('registrado','validado','anulado'))
);
GO

/* ==========================
   5. INVENTARIO DE MATERIALES
   ========================== */
CREATE TABLE materiales (
    id_material INT IDENTITY(1,1) PRIMARY KEY,
    nombre_material VARCHAR(120) NOT NULL,
    descripcion VARCHAR(200) NULL,
    stock_actual DECIMAL(10,2) NOT NULL DEFAULT 0,
    stock_minimo DECIMAL(10,2) NOT NULL DEFAULT 0,
    unidad_medida VARCHAR(20) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'activo',
    fecha_registro DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT ck_materiales_stock_actual CHECK (stock_actual >= 0),
    CONSTRAINT ck_materiales_stock_minimo CHECK (stock_minimo >= 0),
    CONSTRAINT ck_materiales_estado CHECK (estado IN ('activo','inactivo')),
    CONSTRAINT ck_materiales_unidad CHECK (unidad_medida IN ('unidad','metro','hoja','kg','litro','paquete'))
);
GO

CREATE TABLE movimientos_inventario (
    id_movimiento INT IDENTITY(1,1) PRIMARY KEY,
    id_material INT NOT NULL,
    tipo_movimiento VARCHAR(20) NOT NULL,
    cantidad DECIMAL(10,2) NOT NULL,
    motivo VARCHAR(150) NULL,
    referencia VARCHAR(100) NULL,
    fecha_movimiento DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    id_usuario INT NULL,
    CONSTRAINT fk_movimientos_material FOREIGN KEY (id_material) REFERENCES materiales(id_material),
    CONSTRAINT fk_movimientos_usuario FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario),
    CONSTRAINT ck_movimientos_tipo CHECK (tipo_movimiento IN ('entrada','salida','ajuste')),
    CONSTRAINT ck_movimientos_cantidad CHECK (cantidad > 0)
);
GO

CREATE TABLE consumo_materiales (
    id_consumo INT IDENTITY(1,1) PRIMARY KEY,
    id_material INT NOT NULL,
    id_pedido INT NOT NULL,
    cantidad_usada DECIMAL(10,2) NOT NULL,
    fecha_registro DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    observacion VARCHAR(200) NULL,
    CONSTRAINT fk_consumo_materiales_material FOREIGN KEY (id_material) REFERENCES materiales(id_material),
    CONSTRAINT fk_consumo_materiales_pedido FOREIGN KEY (id_pedido) REFERENCES pedidos(id_pedido),
    CONSTRAINT ck_consumo_materiales_cantidad CHECK (cantidad_usada > 0)
);
GO

/* ==========================
   6. CONTACTO
   ========================== */
CREATE TABLE contactos (
    id_contacto INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(120) NOT NULL,
    correo VARCHAR(150) NOT NULL,
    telefono VARCHAR(20) NULL,
    asunto VARCHAR(120) NOT NULL,
    mensaje VARCHAR(500) NOT NULL,
    fecha_registro DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    estado VARCHAR(20) NOT NULL DEFAULT 'pendiente',
    CONSTRAINT ck_contactos_estado CHECK (estado IN ('pendiente','atendido','cerrado'))
);
GO

/* ==========================
   7. ÍNDICES
   ========================== */
CREATE INDEX ix_usuarios_correo ON usuarios(correo);
CREATE INDEX ix_clientes_correo ON clientes(correo);
CREATE INDEX ix_productos_categoria ON productos(id_categoria);
CREATE INDEX ix_pedidos_cliente ON pedidos(id_cliente);
CREATE INDEX ix_pedidos_estado ON pedidos(estado_pedido);
CREATE INDEX ix_pedido_detalles_pedido ON pedido_detalles(id_pedido);
CREATE INDEX ix_pagos_pedido ON pagos(id_pedido);
CREATE INDEX ix_consumo_materiales_pedido ON consumo_materiales(id_pedido);
CREATE INDEX ix_movimientos_material ON movimientos_inventario(id_material);
GO

/* ==========================
   8. PROCEDIMIENTOS ALMACENADOS
   ========================== */
CREATE OR ALTER PROCEDURE sp_recalcular_monto_pedido
    @id_pedido INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @monto_total DECIMAL(10,2) = ISNULL((
        SELECT SUM(cantidad * precio_unitario)
        FROM pedido_detalles
        WHERE id_pedido = @id_pedido
    ),0);

    DECLARE @total_pagado DECIMAL(10,2) = ISNULL((
        SELECT SUM(monto)
        FROM pagos
        WHERE id_pedido = @id_pedido
          AND estado_pago <> 'anulado'
    ),0);

    UPDATE pedidos
    SET monto_total = @monto_total,
        saldo_pendiente = CASE 
                            WHEN @monto_total - @total_pagado < 0 THEN 0
                            ELSE @monto_total - @total_pagado
                          END
    WHERE id_pedido = @id_pedido;
END;
GO

CREATE OR ALTER PROCEDURE sp_registrar_pago
    @id_pedido INT,
    @monto DECIMAL(10,2),
    @metodo_pago VARCHAR(30),
    @tipo_pago VARCHAR(20),
    @comprobante VARCHAR(255) = NULL,
    @observacion VARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO pagos(id_pedido, monto, metodo_pago, comprobante, tipo_pago, estado_pago, observacion)
    VALUES(@id_pedido, @monto, @metodo_pago, @comprobante, @tipo_pago, 'registrado', @observacion);

    EXEC sp_recalcular_monto_pedido @id_pedido;
END;
GO

CREATE OR ALTER PROCEDURE sp_actualizar_stock_material
    @id_material INT,
    @tipo_movimiento VARCHAR(20),
    @cantidad DECIMAL(10,2),
    @motivo VARCHAR(150),
    @referencia VARCHAR(100) = NULL,
    @id_usuario INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @tipo_movimiento = 'entrada'
        UPDATE materiales SET stock_actual = stock_actual + @cantidad WHERE id_material = @id_material;
    ELSE IF @tipo_movimiento IN ('salida','ajuste')
        UPDATE materiales SET stock_actual = stock_actual - @cantidad WHERE id_material = @id_material;

    INSERT INTO movimientos_inventario(id_material, tipo_movimiento, cantidad, motivo, referencia, id_usuario)
    VALUES(@id_material, @tipo_movimiento, @cantidad, @motivo, @referencia, @id_usuario);
END;
GO

/* ==========================
   9. TRIGGERS
   ========================== */
CREATE OR ALTER TRIGGER trg_pedidos_fecha_actualizacion
ON pedidos
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE p
    SET fecha_actualizacion = SYSDATETIME()
    FROM pedidos p
    INNER JOIN inserted i ON p.id_pedido = i.id_pedido;
END;
GO

CREATE OR ALTER TRIGGER trg_pedido_detalles_recalcular
ON pedido_detalles
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Pedidos TABLE(id_pedido INT PRIMARY KEY);

    INSERT INTO @Pedidos(id_pedido)
    SELECT DISTINCT id_pedido FROM inserted WHERE id_pedido IS NOT NULL
    UNION
    SELECT DISTINCT id_pedido FROM deleted WHERE id_pedido IS NOT NULL;

    DECLARE @id_pedido INT;

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT id_pedido FROM @Pedidos;

    OPEN cur;
    FETCH NEXT FROM cur INTO @id_pedido;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        EXEC sp_recalcular_monto_pedido @id_pedido;
        FETCH NEXT FROM cur INTO @id_pedido;
    END

    CLOSE cur;
    DEALLOCATE cur;
END;
GO

CREATE OR ALTER TRIGGER trg_pagos_recalcular_saldo
ON pagos
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Pedidos TABLE(id_pedido INT PRIMARY KEY);

    INSERT INTO @Pedidos(id_pedido)
    SELECT DISTINCT id_pedido FROM inserted WHERE id_pedido IS NOT NULL
    UNION
    SELECT DISTINCT id_pedido FROM deleted WHERE id_pedido IS NOT NULL;

    DECLARE @id_pedido INT;

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT id_pedido FROM @Pedidos;

    OPEN cur;
    FETCH NEXT FROM cur INTO @id_pedido;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        EXEC sp_recalcular_monto_pedido @id_pedido;
        FETCH NEXT FROM cur INTO @id_pedido;
    END

    CLOSE cur;
    DEALLOCATE cur;
END;
GO

/* ==========================
   10. DATOS INICIALES
   ========================== */
INSERT INTO roles(nombre_rol, descripcion) VALUES
('administrador','Control total del sistema'),
('cliente','Usuario comprador del sistema');
GO

INSERT INTO usuarios(nombre, apellido, correo, password_hash, estado, id_rol) VALUES
('Angel','Mananita','admin@mubi.com','HASH_DE_EJEMPLO','activo',1),
('Jackeline','Advincula','cliente1@mubi.com','HASH_DE_EJEMPLO','activo',2),
('Rosa','Sanchez','cliente2@mubi.com','HASH_DE_EJEMPLO','activo',2);
GO

INSERT INTO clientes(nombres, apellidos, correo, telefono, direccion, documento_identidad, id_usuario) VALUES
('Jackeline','Advincula','cliente1@mubi.com','987654321','Capac Yupanqui N°331','71234567',2),
('Rosa','Sanchez','cliente2@mubi.com','956123478','Jr. Ucayali 456','74561234',3),
('Carlos','Ruiz','cliente3@mubi.com','978451236','Av. Miraflores 220','70111222',NULL);
GO

INSERT INTO categorias(nombre_categoria, descripcion, estado) VALUES
('Urbanos','Polos urbanos personalizados','activo'),
('Anime','Polos estilo anime','activo'),
('Pareja','Polos personalizados para parejas','activo'),
('Deportivos Sublimados','Polos sublimados deportivos','activo'),
('Escolares','Polos personalizados para escolares','activo'),
('Publicitarios','Polos para publicidad, campañas y eventos','activo');
GO

INSERT INTO productos(nombre, descripcion, precio, disponibilidad, permite_personalizacion, id_categoria) VALUES
('Polo básico sublimado','Polo blanco para sublimación talla estándar',25.00,'disponible',1,1),
('Polo premium personalizado','Polo algodón premium para diseño personalizado',35.00,'disponible',1,2),
('Polo promocional empresas','Polo por mayor para campañas empresariales',28.50,'disponible',1,3);
GO

INSERT INTO producto_imagenes(id_producto, ruta_imagen, nombre_archivo, descripcion, es_principal) VALUES
(1,'/img/productos/polo1.png','polo1.png','Vista principal polo sublimado',1),
(2,'/img/productos/polo2.png','polo2.png','Vista principal polo premium',1),
(3,'/img/productos/polo3.png','polo3.png','Vista principal polo promocional',1);
GO

INSERT INTO materiales(nombre_material, descripcion, stock_actual, stock_minimo, unidad_medida, estado) VALUES
('Tela poliéster','Tela para polos sublimados',120,20,'metro','activo'),
('Vinil textil','Material para personalización',50,10,'metro','activo'),
('Tinta sublimación','Tinta para impresión',15,5,'litro','activo'),
('Papel transfer','Papel especial para sublimado',200,40,'hoja','activo');
GO

INSERT INTO pedidos(id_cliente, estado_pedido, monto_total, saldo_pendiente, observaciones, direccion_entrega, requiere_delivery) VALUES
(1,'pendiente',0,0,'Pedido para evento escolar','Capac Yupanqui N°331',1),
(2,'en_proceso',0,0,'Diseño personalizado con logo','Jr. Ucayali 456',0),
(3,'entregado',0,0,'Pedido promocional pequeño','Av. Miraflores 220',1);
GO

INSERT INTO pedido_detalles(id_pedido, id_producto, talla, color, cantidad, precio_unitario, diseno_personalizado, archivo_diseno, descripcion_diseno) VALUES
(1,2,'M','Negro',2,35.00,'Logo promoción colegio','/disenos/diseno1.png','Logo frontal y texto posterior'),
(1,1,'L','Blanco',1,25.00,'Frase personalizada','/disenos/diseno2.png','Frase simple'),
(2,2,'L','Azul',2,35.00,'Logo empresa','/disenos/diseno3.png','Logo al pecho'),
(3,3,'M','Rojo',2,28.50,'Campaña local','/disenos/diseno4.png','Diseño para campaña');
GO

INSERT INTO pagos(id_pedido, monto, metodo_pago, comprobante, tipo_pago, estado_pago, observacion) VALUES
(1,30.00,'Yape','/comprobantes/pago1.jpg','adelanto','validado','Adelanto inicial'),
(2,40.00,'Plin','/comprobantes/pago2.jpg','adelanto','validado','Adelanto registrado'),
(3,57.00,'efectivo',NULL,'pago_final','validado','Pago completo al entregar');
GO

INSERT INTO historial_estados_pedido(id_pedido, estado_anterior, estado_nuevo, comentario, id_usuario) VALUES
(1,NULL,'pendiente','Creación del pedido',2),
(2,NULL,'pendiente','Creación del pedido',3),
(2,'pendiente','en_proceso','Se inició producción',1),
(3,NULL,'pendiente','Creación del pedido',NULL),
(3,'pendiente','entregado','Pedido completado',1);
GO

INSERT INTO consumo_materiales(id_material, id_pedido, cantidad_usada, observacion) VALUES
(1,1,4,'Tela usada para 3 polos'),
(2,1,1,'Vinil para personalización'),
(3,2,0.5,'Tinta usada en producción'),
(4,2,4,'Papel usado para diseños');
GO

INSERT INTO movimientos_inventario(id_material, tipo_movimiento, cantidad, motivo, referencia, id_usuario) VALUES
(1,'salida',4,'Producción pedido','PED-1',1),
(2,'salida',1,'Producción pedido','PED-1',1),
(3,'salida',0.5,'Producción pedido','PED-2',1),
(4,'salida',4,'Producción pedido','PED-2',1),
(1,'entrada',20,'Compra proveedor','COMP-001',1);
GO

/* Recalcular montos y saldos iniciales */
EXEC sp_recalcular_monto_pedido 1;
EXEC sp_recalcular_monto_pedido 2;
EXEC sp_recalcular_monto_pedido 3;
GO

/* ==========================
   11. VISTAS DE APOYO Y REPORTES
   ========================== */
CREATE VIEW v_clientes_frecuentes AS
SELECT 
    c.id_cliente,
    CONCAT(c.nombres,' ',c.apellidos) AS cliente,
    COUNT(p.id_pedido) AS total_pedidos,
    ISNULL(SUM(p.monto_total),0) AS monto_acumulado
FROM clientes c
LEFT JOIN pedidos p ON c.id_cliente = p.id_cliente
GROUP BY c.id_cliente, c.nombres, c.apellidos;
GO

CREATE VIEW v_pagos_por_pedido AS
SELECT 
    p.id_pedido,
    CONCAT(c.nombres,' ',c.apellidos) AS cliente,
    p.estado_pedido,
    p.monto_total,
    ISNULL(SUM(pg.monto),0) AS total_pagado,
    p.saldo_pendiente
FROM pedidos p
INNER JOIN clientes c ON p.id_cliente = c.id_cliente
LEFT JOIN pagos pg ON p.id_pedido = pg.id_pedido AND pg.estado_pago <> 'anulado'
GROUP BY p.id_pedido, c.nombres, c.apellidos, p.estado_pedido, p.monto_total, p.saldo_pendiente;
GO

CREATE VIEW v_stock_bajo_materiales AS
SELECT 
    id_material,
    nombre_material,
    stock_actual,
    stock_minimo,
    unidad_medida,
    CASE WHEN stock_actual <= stock_minimo THEN 'ALERTA' ELSE 'OK' END AS estado_stock
FROM materiales;
GO

CREATE VIEW v_ventas_por_fecha AS
SELECT 
    CAST(fecha_pedido AS DATE) AS fecha,
    COUNT(id_pedido) AS total_pedidos,
    SUM(monto_total) AS total_ventas
FROM pedidos
WHERE estado_pedido <> 'cancelado'
GROUP BY CAST(fecha_pedido AS DATE);
GO

/* ==========================
   12. CONSULTAS DE PRUEBA
   ========================== */
SELECT * FROM roles;
SELECT * FROM usuarios;
SELECT * FROM clientes;
SELECT * FROM categorias;
SELECT * FROM productos;
SELECT * FROM pedidos;
SELECT * FROM pedido_detalles;
SELECT * FROM pagos;
SELECT * FROM materiales;
SELECT * FROM v_clientes_frecuentes;
SELECT * FROM v_pagos_por_pedido;
SELECT * FROM v_stock_bajo_materiales;
SELECT * FROM v_ventas_por_fecha;
GO
