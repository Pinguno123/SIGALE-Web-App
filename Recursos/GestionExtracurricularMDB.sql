Use Master;
Go
If Not Exists (Select Name From Sys.Databases Where Name = N'GestionExtracurricularMDB')
Begin
    Create Database GestionExtracurricularMDB;
End;
Go

-- 2. Creación De Logins
Use Master;
Go

-- Logins Administrativos / De Sistema
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_app_backend')
    Create Login [login_app_backend] With Password = 'qwerty', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_douglas')
    Create Login [login_douglas] With Password = 'douglas', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_angie')
    Create Login [login_angie] With Password = 'angie', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_steven')
    Create Login [login_steven] With Password = 'steven', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_rodrigo')
    Create Login [login_rodrigo] With Password = 'rodrigo', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;

-- Módulo DAC
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_dac_admin')
    Create Login [login_dac_admin] With Password = 'dac_admin', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_dac_reader')
    Create Login [login_dac_reader] With Password = 'dac_reader', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;

-- Módulo Deportes
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_deportes_admin')
    Create Login [login_deportes_admin] With Password = 'deportes_admin', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_deportes_reader')
    Create Login [login_deportes_reader] With Password = 'deportes_reader', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;

-- Módulo Asociaciones
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_asociaciones_admin')
    Create Login [login_asociaciones_admin] With Password = 'asociaciones_admin', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_asociaciones_reader')
    Create Login [login_asociaciones_reader] With Password = 'asociaciones_reader', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;

-- Módulo Cursos
If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_cursos_admin')
    Create Login [login_cursos_admin] With Password = 'cursos_admin', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;

If Not Exists (Select Name From Sys.Server_Principals Where Name = 'login_cursos_reader')
    Create Login [login_cursos_reader] With Password = 'cursos_reader', Default_Database = [GestionExtracurricularMDB], Check_Policy = Off;
Go

-- Creación de los users
Use GestionExtracurricularMDB;
-- Usuarios corrientes
Create User [user_app_backend] For Login [login_app_backend];
Create User [user_douglas] For Login [login_douglas];
Create User [user_angie] For Login [login_angie];
Create User [user_steven] For Login [login_steven];
Create User [user_rodrigo] For Login [login_rodrigo];
-- Módulo dac
Create User [user_dac_admin] For Login [login_dac_admin];
Create User [user_dac_reader] For Login [login_dac_reader];
-- Módulo Deportes
Create User [user_deportes_admin] For Login [login_deportes_admin];
Create User [user_deportes_reader] For Login [login_deportes_reader];
-- Módulo Asociaciones
Create User [user_asociaciones_admin] For Login [login_asociaciones_admin];
Create User [user_asociaciones_reader] For Login [login_asociaciones_reader];
-- Módulo Cursos
Create User [user_cursos_admin] For Login [login_cursos_admin];
Create User [user_cursos_reader] For Login [login_cursos_reader];
Go

-- Creación de los roles
Create Role [rol_app_backend];
Create Role [rol_auditor];
Create Role [rol_secretaria];
Create Role [rol_soporte_it];
Create Role [rol_administrador_dac];
Create Role [rol_lector_dac];
Create Role [rol_administrador_deportes];
Create Role [rol_lector_deportes];
Create Role [rol_administrador_asociaciones];
Create Role [rol_lector_asociaciones];
Create Role [rol_administrador_cursos];
Create Role [rol_lector_cursos];
Go

-- Asignar usuarios a los roles
-- Rol app_backend
Alter Role [rol_app_backend] Add Member [user_app_backend];
-- Rol auditor
Alter Role [rol_auditor] Add Member [user_angie];
-- Rol secretaria
Alter Role [rol_secretaria] Add Member [user_steven];
-- Rol soporte_it
Alter Role [rol_soporte_it] Add Member [user_douglas];

-- Módulo dac
-- Rol administrador_dac
Alter Role [rol_administrador_dac] Add Member [user_dac_admin];
-- Rol lector_dac
Alter Role [rol_lector_dac] Add Member [user_dac_reader];

-- Módulo deportes
-- Rol administrador_deportes
Alter Role [rol_administrador_deportes] Add Member [user_deportes_admin];
-- Rol lector_deportes
Alter Role [rol_lector_deportes] Add Member [user_deportes_reader];

-- Módulo asociaciones
-- Rol administrador_asociaciones
Alter Role [rol_administrador_asociaciones] Add Member [user_asociaciones_admin];
-- Rol lector_asociaciones
Alter Role [rol_lector_asociaciones] Add Member [user_asociaciones_reader];

-- Módulo cursos
-- Rol administrador_cursos
Alter Role [rol_administrador_cursos] Add Member [user_cursos_admin];
-- Rol lector_cursos
Alter Role [rol_lector_cursos] Add Member [user_cursos_reader];
Go

-- Creación de esquemas
Create Schema dac; -- DAC
Go

Create Schema auth; -- seguridad
Go

Create Schema sports; -- deportes
Go

Create Schema courses; -- cursos
Go

Create Schema clubs; -- asociaciones
Go

-- Permisos para los roles en los diferentes esquemas
-- Rol app_backend
Grant Select, Insert, Update, Delete On Schema::[dac] To [rol_app_backend];
Grant Select, Insert, Update, Delete On Schema::[auth] To [rol_app_backend];
Grant Select, Insert, Update, Delete On Schema::[sports] To [rol_app_backend];
Grant Select, Insert, Update, Delete On Schema::[courses] To [rol_app_backend];
Grant Select, Insert, Update, Delete On Schema::[clubs] To [rol_app_backend];

-- Rol auditor
Grant Select On Schema::[dac] To [rol_auditor];
Grant Select On Schema::[auth] To [rol_auditor];
Grant Select On Schema::[sports] To [rol_auditor];
Grant Select On Schema::[courses] To [rol_auditor];
Grant Select On Schema::[clubs] To [rol_auditor];

-- Rol secretaria
Grant Select, Insert, Update On Schema::[dac] To [rol_secretaria];
Grant Select On Schema::[auth] To [rol_secretaria];
Grant Select, Insert, Update On Schema::[sports] To [rol_secretaria];
Grant Select, Insert, Update On Schema::[courses] To [rol_secretaria];
Grant Select, Insert, Update On Schema::[clubs] To [rol_secretaria];

-- Rol soporte_it
Grant Select, Insert, Update, Delete On Schema::[dac] To [rol_soporte_it];
Grant Select On Schema::[auth] To [rol_soporte_it];
Grant Select, Insert, Update, Delete On Schema::[sports] To [rol_soporte_it];
Grant Select, Insert, Update, Delete On Schema::[courses] To [rol_soporte_it];
Grant Select, Insert, Update, Delete On Schema::[clubs] To [rol_soporte_it];

-- Módulo dac
-- Rol administrador_dac
Grant Select, Insert, Update, Delete On Schema::[dac] To [rol_administrador_dac];
Grant Select On Schema::[auth] To [rol_administrador_dac];
-- Rol lector_dac
Grant Select On Schema::[dac] To [rol_lector_dac];
Grant Select On Schema::[auth] To [rol_lector_dac];

-- Módulo deportes
-- Rol administrador_deportes
Grant Select On Schema::[auth] To [rol_administrador_deportes];
Grant Select, Insert, Update, Delete On Schema::[sports] To [rol_administrador_deportes];
-- Rol lector_deportes
Grant Select On Schema::[auth] To [rol_lector_deportes];
Grant Select On Schema::[sports] To [rol_lector_deportes];

-- Módulo asociaciones
-- Rol administrador_asociaciones
Grant Select On Schema::[auth] To [rol_administrador_asociaciones];
Grant Select, Insert, Update, Delete On Schema::[clubs] To [rol_administrador_asociaciones];
-- Rol lector_asociaciones
Grant Select On Schema::[auth] To [rol_lector_asociaciones];
Grant Select On Schema::[clubs] To [rol_lector_asociaciones];

-- Módulo cursos
-- Rol administrador_cursos
Grant Select On Schema::[auth] To [rol_administrador_cursos];
Grant Select, Insert, Update, Delete On Schema::[courses] To [rol_administrador_cursos];
-- Rol lector_cursos
Grant Select On Schema::[auth] To [rol_lector_cursos];
Grant Select On Schema::[courses] To [rol_lector_cursos];
Go

-- Creación de las tablas
-- Tablas del esquema auth
CREATE TABLE auth.Roles (
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol VARCHAR(50) NOT NULL UNIQUE,
    Descripcion VARCHAR(250) NULL
);

CREATE TABLE auth.Usuarios (
    Carnet VARCHAR(10) PRIMARY KEY,
    NombreCompleto VARCHAR(100) NOT NULL,
    Carrera VARCHAR(150) NOT NULL,
    Campus VARCHAR(100) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CorreoElectronico VARCHAR(50) NOT NULL,
    IdRol INT NOT NULL,
    FOREIGN KEY (IdRol) REFERENCES auth.Roles(IdRol)
);
GO

-- Creación de la función fn_ValidarInstructor
CREATE FUNCTION auth.fn_ValidarInstructor(@Carnet VARCHAR(10))
RETURNS BIT
AS
BEGIN
    DECLARE @EsValido BIT = 0;
    IF EXISTS (
        SELECT 1 
        FROM auth.Usuarios u
        JOIN auth.Roles r ON u.IdRol = r.IdRol
        WHERE u.Carnet = @Carnet 
          AND u.Activo = 1 
          AND r.NombreRol = 'Instructor'
    )
    BEGIN
        SET @EsValido = 1;
    END
    RETURN @EsValido;
END;
GO

-- Tablas del esquema clubs
CREATE TABLE clubs.Asociaciones (
    IdAsociacion INT IDENTITY(1,1) PRIMARY KEY,
    Acronimo VARCHAR(20) NOT NULL,
    Nombre VARCHAR(200) NOT NULL,
    AnioFundacion INT,
    Descripcion NVARCHAR(MAX),
    Formulario NVARCHAR(MAX),
    ImgUrl NVARCHAR(MAX)
);

CREATE TABLE clubs.MembresiaAsociaciones (
    IdMembresia INT IDENTITY(1,1) PRIMARY KEY,
    Carnet VARCHAR(10) NOT NULL,
    IdAsociacion INT NOT NULL,
    Rol VARCHAR(50) DEFAULT 'Miembro', -- Rol
    FechaSolicitud DATE DEFAULT GETDATE(),
    EstadoValidacion VARCHAR(20) DEFAULT 'Pendiente', -- Estado
    CONSTRAINT CK_RolMembresia CHECK (Rol IN ('Miembro', 'Directiva', 'Presidente', 'Tesorero', 'Secretario')),
    CONSTRAINT CK_EstadoValidacion CHECK (EstadoValidacion IN ('Pendiente', 'Aprobada', 'Rechazada')),
    CONSTRAINT UQ_Estudiante_Asociacion UNIQUE (Carnet, IdAsociacion),
    FOREIGN KEY (Carnet) REFERENCES auth.Usuarios(Carnet),
    FOREIGN KEY (IdAsociacion) REFERENCES clubs.Asociaciones(IdAsociacion)
);
Go

-- Tablas del esquema courses
CREATE TABLE courses.Cursos (
    IdCurso INT IDENTITY(1,1) PRIMARY KEY, -- ID autoincrementable
    NombreCurso VARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(500),
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    Horario VARCHAR(100) NOT NULL,
    CupoMaximo INT NOT NULL,
    Estado VARCHAR(20) DEFAULT 'Activo',
    CarnetInstructor VARCHAR(10) NULL,
    CONSTRAINT CK_FechasCurso CHECK (FechaFin >= FechaInicio),
    CONSTRAINT CK_Curso_InstructorValido CHECK (CarnetInstructor IS NULL OR auth.fn_ValidarInstructor(CarnetInstructor) = 1),
    CONSTRAINT CK_EstadoCurso CHECK (Estado IN ('Activo', 'Inactivo', 'Finalizado')),
    FOREIGN KEY (CarnetInstructor) REFERENCES auth.Usuarios(Carnet)
);

CREATE TABLE courses.InscripcionCursos (
    IdInscripcion INT IDENTITY(1,1) PRIMARY KEY,
    Carnet VARCHAR(10) NOT NULL,
    IdCurso INT NOT NULL,
    Nota DECIMAL(4,2) NULL, -- Nota
    FechaInscripcion DATETIME DEFAULT GETDATE(),
    EstadoInscripcion VARCHAR(20) DEFAULT 'Confirmada',
    CONSTRAINT CK_EstadoInscripcion CHECK (EstadoInscripcion IN ('Confirmada', 'En Espera', 'Cancelada')),
    CONSTRAINT UQ_Estudiante_Curso UNIQUE (Carnet, IdCurso),
    FOREIGN KEY (Carnet) REFERENCES auth.Usuarios(Carnet),
    FOREIGN KEY (IdCurso) REFERENCES courses.Cursos(IdCurso)
);
Go

-- Tablas del esquema sports
CREATE TABLE sports.SeleccionesDeportivas (
    IdSeleccion INT IDENTITY(1,1) PRIMARY KEY,
    Disciplina VARCHAR(50) NOT NULL, -- Disciplina
    Rama VARCHAR(20) NOT NULL,       -- Rama
    NombreEquipo VARCHAR(100) NOT NULL,
    ImgUrl NVARCHAR(MAX)
);

CREATE TABLE sports.ParticipacionDeportiva (
    IdParticipacion INT IDENTITY(1,1) PRIMARY KEY,
    Carnet VARCHAR(10) NOT NULL,
    IdSeleccion INT NOT NULL,
    Modalidad VARCHAR(100), -- Modalidad
    FechaIngreso DATE DEFAULT GETDATE(),
    CONSTRAINT UQ_Estudiante_Seleccion UNIQUE (Carnet, IdSeleccion),
    FOREIGN KEY (Carnet) REFERENCES auth.Usuarios(Carnet),
    FOREIGN KEY (IdSeleccion) REFERENCES sports.SeleccionesDeportivas(IdSeleccion)
);
Go

-- Tablas del esquema dac
CREATE TABLE dac.CategoriasDAC (
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    NombreCategoria VARCHAR(100) NOT NULL, -- Categoria
    Descripcion NVARCHAR(500)
);

CREATE TABLE dac.ActividadesDAC (
    IdActividad INT IDENTITY(1,1) PRIMARY KEY,
    IdCategoria INT NOT NULL,
    NombreActividad VARCHAR(150) NOT NULL, -- Actividad
    Descripcion NVARCHAR(MAX),
    Estado VARCHAR(20) DEFAULT 'Activo',
    CarnetInstructor VARCHAR(10) NULL,
    CONSTRAINT CK_Actividad_InstructorValido CHECK (CarnetInstructor IS NULL OR auth.fn_ValidarInstructor(CarnetInstructor) = 1),
    CONSTRAINT CK_EstadoActividadDAC CHECK (Estado IN ('Activo', 'Inactivo')),
    FOREIGN KEY (IdCategoria) REFERENCES dac.CategoriasDAC(IdCategoria),
    FOREIGN KEY (CarnetInstructor) REFERENCES auth.Usuarios(Carnet)
);

CREATE TABLE dac.InscripcionesDAC (
    IdInscripcionDAC INT IDENTITY(1,1) PRIMARY KEY,
    Carnet VARCHAR(10) NOT NULL,
    IdActividad INT NOT NULL,
    FechaInscripcion DATETIME DEFAULT GETDATE(),
    EstadoInscripcion VARCHAR(20) DEFAULT 'Activa', -- Estado
    CONSTRAINT CK_EstadoInscripcionDAC CHECK (EstadoInscripcion IN ('Activa', 'Finalizada', 'Retirada')),
    CONSTRAINT UQ_Estudiante_ActividadDAC UNIQUE (Carnet, IdActividad),
    FOREIGN KEY (Carnet) REFERENCES auth.Usuarios(Carnet),
    FOREIGN KEY (IdActividad) REFERENCES dac.ActividadesDAC(IdActividad)
);
GO

-- Creación de los triggers
-- Trigger de inserción
CREATE TRIGGER TR_InscripcionCursos_Insert
ON courses.InscripcionCursos
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Inserta registros
    DECLARE @Carnet VARCHAR(10), @IdCurso INT, @Nota DECIMAL(4,2), @FechaInscripcion DATETIME, @EstadoInscripcion VARCHAR(20);
    
    DECLARE cursor_inscripciones CURSOR LOCAL FOR 
    SELECT Carnet, IdCurso, Nota, FechaInscripcion, EstadoInscripcion FROM inserted;
    
    OPEN cursor_inscripciones;
    FETCH NEXT FROM cursor_inscripciones INTO @Carnet, @IdCurso, @Nota, @FechaInscripcion, @EstadoInscripcion;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE @CupoMaximo INT;
        DECLARE @InscritosActivos INT;
        
        -- Obtiene cupo
        SELECT @CupoMaximo = CupoMaximo FROM courses.Cursos WHERE IdCurso = @IdCurso;
        
        -- Obtiene inscritos
        SELECT @InscritosActivos = COUNT(*) FROM courses.InscripcionCursos 
        WHERE IdCurso = @IdCurso AND EstadoInscripcion = 'Confirmada';
        
        -- Define estado
        DECLARE @EstadoFinal VARCHAR(20) = COALESCE(@EstadoInscripcion, 'Confirmada');
        
        IF @EstadoFinal = 'Confirmada' AND @InscritosActivos >= @CupoMaximo
        BEGIN
            SET @EstadoFinal = 'En Espera';
        END
        
        -- Inserta
        INSERT INTO courses.InscripcionCursos (Carnet, IdCurso, Nota, FechaInscripcion, EstadoInscripcion)
        VALUES (@Carnet, @IdCurso, @Nota, COALESCE(@FechaInscripcion, GETDATE()), @EstadoFinal);
        
        FETCH NEXT FROM cursor_inscripciones INTO @Carnet, @IdCurso, @Nota, @FechaInscripcion, @EstadoInscripcion;
    END
    
    CLOSE cursor_inscripciones;
    DEALLOCATE cursor_inscripciones;
END;
GO

-- Trigger de actualización y borrado
CREATE TRIGGER TR_InscripcionCursos_UpdateDelete
ON courses.InscripcionCursos
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF TRIGGER_NESTLEVEL() > 1 -- Evita el bucle recursivo
        RETURN;
    
    -- Verifica cupo
    -- Caso 1
    -- Caso 2
    IF EXISTS (
        SELECT 1 
        FROM deleted d
        LEFT JOIN inserted i ON d.IdInscripcion = i.IdInscripcion
        WHERE d.EstadoInscripcion = 'Confirmada' 
          AND (i.IdInscripcion IS NULL OR i.EstadoInscripcion = 'Cancelada')
    )
    BEGIN
        -- Cursos afectados
        DECLARE @IdCursoAfectado INT;
        
        DECLARE cursor_cursos_afectados CURSOR LOCAL FOR
        SELECT DISTINCT d.IdCurso 
        FROM deleted d
        LEFT JOIN inserted i ON d.IdInscripcion = i.IdInscripcion
        WHERE d.EstadoInscripcion = 'Confirmada' 
          AND (i.IdInscripcion IS NULL OR i.EstadoInscripcion = 'Cancelada');
        
        OPEN cursor_cursos_afectados;
        FETCH NEXT FROM cursor_cursos_afectados INTO @IdCursoAfectado;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE @CupoMaximo INT;
            DECLARE @InscritosActivos INT;
            
            SELECT @CupoMaximo = CupoMaximo FROM courses.Cursos WHERE IdCurso = @IdCursoAfectado;
            SELECT @InscritosActivos = COUNT(*) FROM courses.InscripcionCursos
            WHERE IdCurso = @IdCursoAfectado AND EstadoInscripcion = 'Confirmada';
            
            -- Si hay cupos
            WHILE @InscritosActivos < @CupoMaximo
            BEGIN
                DECLARE @SiguienteInscripcionId INT = NULL;
                
                -- Obtiene el ID
                SELECT TOP 1 @SiguienteInscripcionId = IdInscripcion
                FROM courses.InscripcionCursos
                WHERE IdCurso = @IdCursoAfectado AND EstadoInscripcion = 'En Espera'
                ORDER BY FechaInscripcion ASC;
                
                -- Si es nulo, sale
                IF @SiguienteInscripcionId IS NULL
                    BREAK;
                
                -- Actualiza
                UPDATE courses.InscripcionCursos
                SET EstadoInscripcion = 'Confirmada'
                WHERE IdInscripcion = @SiguienteInscripcionId;
                
                -- Recalcula
                SELECT @InscritosActivos = COUNT(*) FROM courses.InscripcionCursos
                WHERE IdCurso = @IdCursoAfectado AND EstadoInscripcion = 'Confirmada';
            END
            
            FETCH NEXT FROM cursor_cursos_afectados INTO @IdCursoAfectado;
        END
        
        CLOSE cursor_cursos_afectados;
        DEALLOCATE cursor_cursos_afectados;
    END
END;
GO

-- Creación de las vistas
-- Vista inscripciones cursos
CREATE VIEW courses.uv_DetalleInscripcionesCursos AS
SELECT 
    i.IdInscripcion,
    u.Carnet,
    u.NombreCompleto AS NombreEstudiante,
    u.Carrera,
    u.Campus,
    c.IdCurso,
    c.NombreCurso,
    c.FechaInicio,
    c.FechaFin,
    c.Horario,
    c.CupoMaximo,
    i.Nota,
    i.FechaInscripcion,
    i.EstadoInscripcion,
    inst.NombreCompleto AS NombreInstructor
FROM courses.InscripcionCursos i
JOIN auth.Usuarios u ON i.Carnet = u.Carnet
JOIN courses.Cursos c ON i.IdCurso = c.IdCurso
LEFT JOIN auth.Usuarios inst ON c.CarnetInstructor = inst.Carnet;
GO

-- Vista membresias
CREATE VIEW clubs.uv_DetalleMembresiasAsociaciones AS
SELECT 
    m.IdMembresia,
    u.Carnet,
    u.NombreCompleto AS NombreEstudiante,
    u.Carrera,
    a.IdAsociacion,
    a.Acronimo,
    a.Nombre AS NombreAsociacion,
    m.Rol,
    m.FechaSolicitud,
    m.EstadoValidacion
FROM clubs.MembresiaAsociaciones m
JOIN auth.Usuarios u ON m.Carnet = u.Carnet
JOIN clubs.Asociaciones a ON m.IdAsociacion = a.IdAsociacion;
GO

-- Vista inscripciones dac
CREATE VIEW dac.uv_DetalleInscripcionesDAC AS
SELECT 
    i.IdInscripcionDAC,
    u.Carnet,
    u.NombreCompleto AS NombreEstudiante,
    u.Carrera,
    a.IdActividad,
    a.NombreActividad,
    c.NombreCategoria,
    i.FechaInscripcion,
    i.EstadoInscripcion,
    inst.NombreCompleto AS NombreInstructor
FROM dac.InscripcionesDAC i
JOIN auth.Usuarios u ON i.Carnet = u.Carnet
JOIN dac.ActividadesDAC a ON i.IdActividad = a.IdActividad
JOIN dac.CategoriasDAC c ON a.IdCategoria = c.IdCategoria
LEFT JOIN auth.Usuarios inst ON a.CarnetInstructor = inst.Carnet;
GO

-- Vista deportes
CREATE VIEW sports.uv_DetalleParticipacionDeportiva AS
SELECT 
    p.IdParticipacion,
    u.Carnet,
    u.NombreCompleto AS NombreEstudiante,
    u.Carrera,
    s.IdSeleccion,
    s.Disciplina,
    s.Rama,
    s.NombreEquipo,
    p.Modalidad,
    p.FechaIngreso
FROM sports.ParticipacionDeportiva p
JOIN auth.Usuarios u ON p.Carnet = u.Carnet
JOIN sports.SeleccionesDeportivas s ON p.IdSeleccion = s.IdSeleccion;
GO

-- Instructores activos
CREATE VIEW auth.uv_ListarInstructores AS
SELECT 
    u.Carnet,
    u.NombreCompleto AS NombreInstructor,
    u.Carrera,
    u.Campus,
    u.Activo,
    u.CorreoElectronico
FROM auth.Usuarios u
JOIN auth.Roles r ON u.IdRol = r.IdRol
WHERE r.NombreRol = 'Instructor';
GO

-- Cursos con cupos
CREATE VIEW courses.uv_ListarCursosConCupos AS
SELECT 
    c.IdCurso,
    c.NombreCurso,
    c.Descripcion,
    c.FechaInicio,
    c.FechaFin,
    c.Horario,
    c.CupoMaximo,
    c.Estado,
    inst.NombreCompleto AS NombreInstructor,
    COUNT(CASE WHEN i.EstadoInscripcion = 'Confirmada' THEN 1 END) AS CuposOcupados,
    c.CupoMaximo - COUNT(CASE WHEN i.EstadoInscripcion = 'Confirmada' THEN 1 END) AS CuposDisponibles
FROM courses.Cursos c
LEFT JOIN auth.Usuarios inst ON c.CarnetInstructor = inst.Carnet
LEFT JOIN courses.InscripcionCursos i ON c.IdCurso = i.IdCurso
GROUP BY c.IdCurso, c.NombreCurso, c.Descripcion, c.FechaInicio, c.FechaFin, c.Horario, c.CupoMaximo, c.Estado, inst.NombreCompleto;
GO

-- Creación de procedimientos almacenados
-- Inscribe a curso
CREATE PROCEDURE courses.sp_InscribirEstudianteCurso
    @Carnet VARCHAR(10),
    @IdCurso INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verifica estudiante
        IF NOT EXISTS (SELECT 1 FROM auth.Usuarios WHERE Carnet = @Carnet)
        BEGIN
            ;THROW 50001, 'El estudiante con el carnet especificado no está registrado en el sistema.', 1;
        END
        
        -- Verifica curso
        IF NOT EXISTS (SELECT 1 FROM courses.Cursos WHERE IdCurso = @IdCurso)
        BEGIN
            ;THROW 50002, 'El curso especificado no existe.', 1;
        END
        
        -- Verifica inscripción
        DECLARE @EstadoActual VARCHAR(20) = NULL;
        DECLARE @IdInscripcion INT = NULL;
        
        SELECT @IdInscripcion = IdInscripcion, @EstadoActual = EstadoInscripcion 
        FROM courses.InscripcionCursos 
        WHERE Carnet = @Carnet AND IdCurso = @IdCurso;
        
        IF @EstadoActual IS NOT NULL
        BEGIN
            -- Si está activo, error
            IF @EstadoActual IN ('Confirmada', 'En Espera')
            BEGIN
                DECLARE @MsgError VARCHAR(150) = 'El estudiante ya cuenta con una inscripción activa en este curso. Estado actual: ' + @EstadoActual;
                ;THROW 50003, @MsgError, 1;
            END
            
            -- Si está cancelado, re-inscribe
            IF @EstadoActual = 'Cancelada'
            BEGIN
                DECLARE @CupoMaximo INT;
                DECLARE @InscritosActivos INT;
                
                SELECT @CupoMaximo = CupoMaximo FROM courses.Cursos WHERE IdCurso = @IdCurso;
                SELECT @InscritosActivos = COUNT(*) FROM courses.InscripcionCursos 
                WHERE IdCurso = @IdCurso AND EstadoInscripcion = 'Confirmada';
                
                DECLARE @NuevoEstado VARCHAR(20) = 'Confirmada';
                
                -- Si no hay cupo, espera
                IF @InscritosActivos >= @CupoMaximo
                BEGIN
                    SET @NuevoEstado = 'En Espera';
                END
                
                UPDATE courses.InscripcionCursos
                SET EstadoInscripcion = @NuevoEstado,
                    FechaInscripcion = GETDATE(),
                    Nota = NULL
                WHERE IdInscripcion = @IdInscripcion;
            END
        END
        ELSE
        BEGIN
            -- Inserta
            INSERT INTO courses.InscripcionCursos (Carnet, IdCurso)
            VALUES (@Carnet, @IdCurso);
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Cancela curso
CREATE PROCEDURE courses.sp_CancelarInscripcionCurso
    @Carnet VARCHAR(10),
    @IdCurso INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verifica inscripción
        IF NOT EXISTS (
            SELECT 1 
            FROM courses.InscripcionCursos 
            WHERE Carnet = @Carnet AND IdCurso = @IdCurso AND EstadoInscripcion IN ('Confirmada', 'En Espera')
        )
        BEGIN
            ;THROW 50004, 'No existe una inscripción activa para el estudiante en el curso especificado.', 1;
        END
        
        -- Actualiza
        UPDATE courses.InscripcionCursos
        SET EstadoInscripcion = 'Cancelada'
        WHERE Carnet = @Carnet AND IdCurso = @IdCurso;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Registra usuario
CREATE PROCEDURE auth.sp_RegistrarUsuario
    @Carnet VARCHAR(10),
    @NombreCompleto VARCHAR(100),
    @Carrera VARCHAR(150),
    @Campus VARCHAR(100),
    @CorreoElectronico VARCHAR(50),
    @NombreRol VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verifica rol
        DECLARE @IdRol INT = NULL;
        SELECT @IdRol = IdRol FROM auth.Roles WHERE NombreRol = @NombreRol;
        
        IF @IdRol IS NULL
        BEGIN
            DECLARE @MsgError VARCHAR(150) = 'El rol especificado (' + COALESCE(@NombreRol, 'NULL') + ') no existe en el sistema.';
            ;THROW 50005, @MsgError, 1;
        END
        
        -- Verifica carnet
        IF EXISTS (SELECT 1 FROM auth.Usuarios WHERE Carnet = @Carnet)
        BEGIN
            ;THROW 50006, 'El carnet ingresado ya se encuentra registrado.', 1;
        END
        
        -- Inserta
        INSERT INTO auth.Usuarios (Carnet, NombreCompleto, Carrera, Campus, Activo, CorreoElectronico, IdRol)
        VALUES (@Carnet, @NombreCompleto, @Carrera, @Campus, 1, @CorreoElectronico, @IdRol);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Crea rol
CREATE PROCEDURE auth.sp_CrearRol
    @NombreRol VARCHAR(50),
    @Descripcion VARCHAR(250)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM auth.Roles WHERE NombreRol = @NombreRol)
            BEGIN
                ;THROW 50007, 'El rol ya existe en el sistema.', 1;
            END
            
        INSERT INTO auth.Roles (NombreRol, Descripcion) 
        VALUES (@NombreRol, @Descripcion);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Actualiza usuario
CREATE PROCEDURE auth.sp_ActualizarUsuario
    @Carnet VARCHAR(10),
    @NombreCompleto VARCHAR(100),
    @Carrera VARCHAR(150),
    @Campus VARCHAR(100),
    @Activo BIT,
    @CorreoElectronico VARCHAR(50),
    @NombreRol VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM auth.Usuarios WHERE Carnet = @Carnet)
            BEGIN
                ;THROW 50008, 'El usuario con el carnet especificado no existe.', 1;
            END
            
        DECLARE @IdRol INT = NULL;
        SELECT @IdRol = IdRol FROM auth.Roles WHERE NombreRol = @NombreRol;
        IF @IdRol IS NULL
            BEGIN
                ;THROW 50009, 'El rol especificado no existe.', 1;
            END
            
        UPDATE auth.Usuarios
        SET NombreCompleto = @NombreCompleto, 
            Carrera = @Carrera, 
            Campus = @Campus,
            Activo = @Activo, 
            CorreoElectronico = @CorreoElectronico, 
            IdRol = @IdRol
        WHERE Carnet = @Carnet;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Crea asociación
CREATE PROCEDURE clubs.sp_CrearAsociacion
    @Acronimo VARCHAR(20),
    @Nombre VARCHAR(200),
    @AnioFundacion INT,
    @Descripcion NVARCHAR(MAX),
    @Formulario NVARCHAR(MAX),
    @ImgUrl NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM clubs.Asociaciones WHERE Acronimo = @Acronimo)
            BEGIN
                ;THROW 50010, 'Ya existe una asociación con ese acrónimo.', 1;
            END
            
        INSERT INTO clubs.Asociaciones (Acronimo, Nombre, AnioFundacion, Descripcion, Formulario, ImgUrl)
        VALUES (@Acronimo, @Nombre, @AnioFundacion, @Descripcion, @Formulario, @ImgUrl);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Crea curso
CREATE PROCEDURE courses.sp_CrearCurso
    @NombreCurso VARCHAR(150),
    @Descripcion NVARCHAR(500),
    @FechaInicio DATE,
    @FechaFin DATE,
    @Horario VARCHAR(100),
    @CupoMaximo INT,
    @CarnetInstructor VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
            
        INSERT INTO courses.Cursos (NombreCurso, Descripcion, FechaInicio, FechaFin, Horario, CupoMaximo, Estado, CarnetInstructor)
        VALUES (@NombreCurso, @Descripcion, @FechaInicio, @FechaFin, @Horario, @CupoMaximo, 'Activo', @CarnetInstructor);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Actualiza curso
CREATE PROCEDURE courses.sp_ActualizarCurso
    @IdCurso INT,
    @NombreCurso VARCHAR(150),
    @Descripcion NVARCHAR(500),
    @FechaInicio DATE,
    @FechaFin DATE,
    @Horario VARCHAR(100),
    @CupoMaximo INT,
    @Estado VARCHAR(20),
    @CarnetInstructor VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM courses.Cursos WHERE IdCurso = @IdCurso)
            BEGIN
                ;THROW 50012, 'El curso especificado no existe.', 1;
            END
            
        UPDATE courses.Cursos
        SET NombreCurso = @NombreCurso, 
            Descripcion = @Descripcion, 
            FechaInicio = @FechaInicio,
            FechaFin = @FechaFin, 
            Horario = @Horario, 
            CupoMaximo = @CupoMaximo, 
            Estado = @Estado,
            CarnetInstructor = @CarnetInstructor
        WHERE IdCurso = @IdCurso;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Crea selección deportiva
CREATE PROCEDURE sports.sp_CrearSeleccionDeportiva
    @Disciplina VARCHAR(50),
    @Rama VARCHAR(20),
    @NombreEquipo VARCHAR(100),
    @ImgUrl NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM sports.SeleccionesDeportivas WHERE NombreEquipo = @NombreEquipo)
            BEGIN
                ;THROW 50013, 'El nombre de equipo deportivo ya se encuentra registrado.', 1;
            END
            
        INSERT INTO sports.SeleccionesDeportivas (Disciplina, Rama, NombreEquipo, ImgUrl)
        VALUES (@Disciplina, @Rama, @NombreEquipo, @ImgUrl);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Crea categoría
CREATE PROCEDURE dac.sp_CrearCategoriaDAC
    @NombreCategoria VARCHAR(100),
    @Descripcion NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM dac.CategoriasDAC WHERE NombreCategoria = @NombreCategoria)
            BEGIN
                ;THROW 50014, 'Ya existe una categoría artística con ese nombre.', 1;
            END
            
        INSERT INTO dac.CategoriasDAC (NombreCategoria, Descripcion)
        VALUES (@NombreCategoria, @Descripcion);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Crea actividad
CREATE PROCEDURE dac.sp_CrearActividadDAC
    @IdCategoria INT,
    @NombreActividad VARCHAR(150),
    @Descripcion NVARCHAR(MAX),
    @CarnetInstructor VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM dac.CategoriasDAC WHERE IdCategoria = @IdCategoria)
            BEGIN
                ;THROW 50015, 'La categoría DAC especificada no existe.', 1;
            END
            
        INSERT INTO dac.ActividadesDAC (IdCategoria, NombreActividad, Descripcion, Estado, CarnetInstructor)
        VALUES (@IdCategoria, @NombreActividad, @Descripcion, 'Activo', @CarnetInstructor);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Inscribe asociación
CREATE PROCEDURE clubs.sp_InscribirMembresiaAsociacion
    @Carnet VARCHAR(10),
    @IdAsociacion INT,
    @Rol VARCHAR(50) = 'Miembro'
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM auth.Usuarios WHERE Carnet = @Carnet)
            BEGIN
                ;THROW 50016, 'El carnet ingresado no existe en el sistema.', 1;
            END
        IF NOT EXISTS (SELECT 1 FROM clubs.Asociaciones WHERE IdAsociacion = @IdAsociacion)
            BEGIN
                ;THROW 50017, 'La asociación especificada no existe.', 1;
            END
            
        DECLARE @EstadoActual VARCHAR(20) = NULL;
        DECLARE @IdMembresia INT = NULL;
        SELECT @IdMembresia = IdMembresia, @EstadoActual = EstadoValidacion 
        FROM clubs.MembresiaAsociaciones WHERE Carnet = @Carnet AND IdAsociacion = @IdAsociacion;
        
        IF @EstadoActual IS NOT NULL
        BEGIN
            IF @EstadoActual IN ('Pendiente', 'Aprobada')
                BEGIN
                    ;THROW 50018, 'El estudiante ya cuenta con una solicitud activa o membresía en esta asociación.', 1;
                END
            IF @EstadoActual = 'Rechazada'
            BEGIN
                UPDATE clubs.MembresiaAsociaciones
                SET EstadoValidacion = 'Pendiente', Rol = COALESCE(@Rol, 'Miembro'), FechaSolicitud = GETDATE()
                WHERE IdMembresia = @IdMembresia;
            END
        END
        ELSE
        BEGIN
            INSERT INTO clubs.MembresiaAsociaciones (Carnet, IdAsociacion, Rol, EstadoValidacion)
            VALUES (@Carnet, @IdAsociacion, COALESCE(@Rol, 'Miembro'), 'Pendiente');
        END
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Cancela asociación
CREATE PROCEDURE clubs.sp_CancelarMembresiaAsociacion
    @Carnet VARCHAR(10),
    @IdAsociacion INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM clubs.MembresiaAsociaciones WHERE Carnet = @Carnet AND IdAsociacion = @IdAsociacion AND EstadoValidacion IN ('Pendiente', 'Aprobada'))
            BEGIN
                ;THROW 50019, 'No existe una membresía activa o pendiente para este estudiante en la asociación.', 1;
            END
            
        UPDATE clubs.MembresiaAsociaciones
        SET EstadoValidacion = 'Rechazada'
        WHERE Carnet = @Carnet AND IdAsociacion = @IdAsociacion;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Inscribe selección
CREATE PROCEDURE sports.sp_InscribirParticipacionDeportiva
    @Carnet VARCHAR(10),
    @IdSeleccion INT,
    @Modalidad VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM auth.Usuarios WHERE Carnet = @Carnet)
            BEGIN
                ;THROW 50020, 'El carnet del estudiante no existe.', 1;
            END
        IF NOT EXISTS (SELECT 1 FROM sports.SeleccionesDeportivas WHERE IdSeleccion = @IdSeleccion)
            BEGIN
                ;THROW 50021, 'La selección deportiva especificada no existe.', 1;
            END
        IF EXISTS (SELECT 1 FROM sports.ParticipacionDeportiva WHERE Carnet = @Carnet AND IdSeleccion = @IdSeleccion)
            BEGIN
                ;THROW 50022, 'El estudiante ya participa en esta selección deportiva.', 1;
            END
            
        INSERT INTO sports.ParticipacionDeportiva (Carnet, IdSeleccion, Modalidad)
        VALUES (@Carnet, @IdSeleccion, @Modalidad);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Cancela selección
CREATE PROCEDURE sports.sp_CancelarParticipacionDeportiva
    @Carnet VARCHAR(10),
    @IdSeleccion INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM sports.ParticipacionDeportiva WHERE Carnet = @Carnet AND IdSeleccion = @IdSeleccion)
            BEGIN
                ;THROW 50023, 'El estudiante no tiene registrada participación en esta selección deportiva.', 1;
            END
            
        DELETE FROM sports.ParticipacionDeportiva
        WHERE Carnet = @Carnet AND IdSeleccion = @IdSeleccion;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Inscribe actividad
CREATE PROCEDURE dac.sp_InscribirActividadDAC
    @Carnet VARCHAR(10),
    @IdActividad INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM auth.Usuarios WHERE Carnet = @Carnet)
            BEGIN
                ;THROW 50024, 'El carnet del estudiante no existe.', 1;
            END
        IF NOT EXISTS (SELECT 1 FROM dac.ActividadesDAC WHERE IdActividad = @IdActividad)
            BEGIN
                ;THROW 50025, 'La actividad artística (DAC) especificada no existe.', 1;
            END
            
        DECLARE @EstadoActual VARCHAR(20) = NULL;
        DECLARE @IdInscripcionDAC INT = NULL;
        SELECT @IdInscripcionDAC = IdInscripcionDAC, @EstadoActual = EstadoInscripcion
        FROM dac.InscripcionesDAC WHERE Carnet = @Carnet AND IdActividad = @IdActividad;
        
        IF @EstadoActual IS NOT NULL
        BEGIN
            IF @EstadoActual IN ('Activa', 'Finalizada')
                BEGIN
                    ;THROW 50026, 'El estudiante ya cuenta con una inscripción activa o finalizada para esta actividad.', 1;
                END
            IF @EstadoActual = 'Retirada'
            BEGIN
                UPDATE dac.InscripcionesDAC
                SET EstadoInscripcion = 'Activa', FechaInscripcion = GETDATE()
                WHERE IdInscripcionDAC = @IdInscripcionDAC;
            END
        END
        ELSE
        BEGIN
            INSERT INTO dac.InscripcionesDAC (Carnet, IdActividad)
            VALUES (@Carnet, @IdActividad);
        END
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO

-- Cancela actividad
CREATE PROCEDURE dac.sp_CancelarInscripcionDAC
    @Carnet VARCHAR(10),
    @IdActividad INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM dac.InscripcionesDAC WHERE Carnet = @Carnet AND IdActividad = @IdActividad AND EstadoInscripcion = 'Activa')
            BEGIN
                ;THROW 50027, 'No existe una inscripción activa para este estudiante en la actividad artística.', 1;
            END
            
        UPDATE dac.InscripcionesDAC
        SET EstadoInscripcion = 'Retirada'
        WHERE Carnet = @Carnet AND IdActividad = @IdActividad;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH
END;
GO