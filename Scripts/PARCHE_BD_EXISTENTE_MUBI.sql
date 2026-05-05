USE MUBI_DB_FINAL;
GO

IF COL_LENGTH('clientes', 'documento_identidad') IS NULL
BEGIN
    ALTER TABLE clientes ADD documento_identidad VARCHAR(20) NULL;
END;
GO

IF EXISTS (
    SELECT 1
    FROM sys.key_constraints
    WHERE parent_object_id = OBJECT_ID('clientes')
      AND type = 'UQ'
      AND name LIKE 'UQ__clientes%'
)
BEGIN
    DECLARE @constraintName SYSNAME;
    SELECT TOP 1 @constraintName = name
    FROM sys.key_constraints
    WHERE parent_object_id = OBJECT_ID('clientes')
      AND type = 'UQ'
      AND name LIKE 'UQ__clientes%';

    DECLARE @sql NVARCHAR(MAX) = N'ALTER TABLE clientes DROP CONSTRAINT ' + QUOTENAME(@constraintName);
    EXEC sp_executesql @sql;
END;
GO

UPDATE categorias SET estado = LOWER(estado) WHERE estado IN ('Activo','Inactivo');
UPDATE usuarios SET estado = LOWER(estado) WHERE estado IN ('Activo','Inactivo','Bloqueado');
UPDATE materiales SET estado = LOWER(estado) WHERE estado IN ('Activo','Inactivo');
UPDATE contactos SET estado = LOWER(estado) WHERE estado IN ('Pendiente','Atendido','Cerrado');
GO

IF NOT EXISTS (SELECT 1 FROM categorias WHERE nombre_categoria = 'Urbanos')
INSERT INTO categorias(nombre_categoria, descripcion, estado) VALUES ('Urbanos','Polos urbanos personalizados','activo');
IF NOT EXISTS (SELECT 1 FROM categorias WHERE nombre_categoria = 'Anime')
INSERT INTO categorias(nombre_categoria, descripcion, estado) VALUES ('Anime','Polos estilo anime','activo');
IF NOT EXISTS (SELECT 1 FROM categorias WHERE nombre_categoria = 'Pareja')
INSERT INTO categorias(nombre_categoria, descripcion, estado) VALUES ('Pareja','Polos personalizados para parejas','activo');
IF NOT EXISTS (SELECT 1 FROM categorias WHERE nombre_categoria = 'Deportivos Sublimados')
INSERT INTO categorias(nombre_categoria, descripcion, estado) VALUES ('Deportivos Sublimados','Polos sublimados deportivos','activo');
IF NOT EXISTS (SELECT 1 FROM categorias WHERE nombre_categoria = 'Escolares')
INSERT INTO categorias(nombre_categoria, descripcion, estado) VALUES ('Escolares','Polos personalizados para escolares','activo');
IF NOT EXISTS (SELECT 1 FROM categorias WHERE nombre_categoria = 'Publicitarios')
INSERT INTO categorias(nombre_categoria, descripcion, estado) VALUES ('Publicitarios','Polos para publicidad, campañas y eventos','activo');
GO
