USE ProyectoVisual;
GO

-- Eliminar constraints y tablas si existen (en orden)
IF OBJECT_ID('dbo.Comentarios','U') IS NOT NULL DROP TABLE dbo.Comentarios;
IF OBJECT_ID('dbo.Publicaciones','U') IS NOT NULL DROP TABLE dbo.Publicaciones;
IF OBJECT_ID('dbo.Usuarios','U') IS NOT NULL DROP TABLE dbo.Usuarios;
IF OBJECT_ID('dbo.Roles','U') IS NOT NULL DROP TABLE dbo.Roles;
GO

-- Crear tabla Roles
CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(200) NULL
);
GO

-- Crear tabla Usuarios
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(150) UNIQUE NOT NULL,
    Contrasena NVARCHAR(255) NOT NULL,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    IdRol INT NOT NULL,
    Estado BIT DEFAULT 1,
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (IdRol) REFERENCES Roles(Id)
);
GO

-- Crear tabla Publicaciones
CREATE TABLE Publicaciones (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario INT NOT NULL,
    Titulo NVARCHAR(200) NOT NULL,
    Contenido NVARCHAR(MAX) NOT NULL,
    FechaPublicacion DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Publicaciones_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(Id)
);
GO

-- Crear tabla Comentarios
CREATE TABLE Comentarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IdPublicacion INT NOT NULL,
    IdUsuario INT NOT NULL,
    Contenido NVARCHAR(MAX) NOT NULL,
    FechaComentario DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Comentarios_Publicaciones FOREIGN KEY (IdPublicacion) REFERENCES Publicaciones(Id),
    CONSTRAINT FK_Comentarios_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(Id)
);
GO

-- Insertar roles base
INSERT INTO Roles (NombreRol, Descripcion)
VALUES 
('Administrador', 'Tiene acceso total al sistema.'),
('Profesor', 'Puede responder a publicaciones y revisar comentarios.'),
('Estudiante', 'Puede publicar y comentar.');
GO

-- Insertar usuarios base
INSERT INTO Usuarios (Nombre, Apellido, Correo, Contrasena, IdRol)
VALUES 
('Juan', 'Pérez', 'admin@upc.edu', 'admin123', 1),
('María', 'Gómez', 'maria@upc.edu', 'profesor123', 2),
('Carlos', 'Ramírez', 'carlos@upc.edu', 'estudiante123', 3);
GO
