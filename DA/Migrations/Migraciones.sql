IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261101220917_Inicial'
)
BEGIN
    CREATE TABLE [Conferencistas] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Correo] nvarchar(150) NOT NULL,
        [Especialidad] nvarchar(100) NOT NULL,
        [Biografia] nvarchar(500) NULL,
        [FechaRegistro] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Conferencistas] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261101220917_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Conferencistas_Correo] ON [Conferencistas] ([Correo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261101220917_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261101220917_Inicial', N'10.0.14');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102012853_Eventos'
)
BEGIN
    CREATE TABLE [Eventos] (
        [Id] int NOT NULL IDENTITY,
        [Titulo] nvarchar(150) NOT NULL,
        [Descripcion] nvarchar(1000) NULL,
        [Lugar] nvarchar(150) NOT NULL,
        [FechaInicio] datetime2 NOT NULL,
        [FechaFin] datetime2 NOT NULL,
        [CupoMaximo] int NOT NULL,
        [Estado] nvarchar(20) NOT NULL,
        [ConferencistaId] int NOT NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Eventos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Eventos_Conferencistas_ConferencistaId] FOREIGN KEY ([ConferencistaId]) REFERENCES [Conferencistas] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102012853_Eventos'
)
BEGIN
    CREATE INDEX [IX_Eventos_ConferencistaId] ON [Eventos] ([ConferencistaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102012853_Eventos'
)
BEGIN
    CREATE INDEX [IX_Eventos_Estado] ON [Eventos] ([Estado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102012853_Eventos'
)
BEGIN
    CREATE INDEX [IX_Eventos_FechaInicio] ON [Eventos] ([FechaInicio]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102012853_Eventos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261102012853_Eventos', N'10.0.14');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102052620_AsistentesEInscripciones'
)
BEGIN
    CREATE TABLE [Asistentes] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Correo] nvarchar(150) NOT NULL,
        [FechaRegistro] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Asistentes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102052620_AsistentesEInscripciones'
)
BEGIN
    CREATE TABLE [Inscripciones] (
        [Id] int NOT NULL IDENTITY,
        [EventoId] int NOT NULL,
        [AsistenteId] int NOT NULL,
        [FechaInscripcion] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Inscripciones] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Inscripciones_Asistentes_AsistenteId] FOREIGN KEY ([AsistenteId]) REFERENCES [Asistentes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Inscripciones_Eventos_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [Eventos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102052620_AsistentesEInscripciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Asistentes_Correo] ON [Asistentes] ([Correo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102052620_AsistentesEInscripciones'
)
BEGIN
    CREATE INDEX [IX_Inscripciones_AsistenteId] ON [Inscripciones] ([AsistenteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102052620_AsistentesEInscripciones'
)
BEGIN
    CREATE INDEX [IX_Inscripciones_EventoId] ON [Inscripciones] ([EventoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102052620_AsistentesEInscripciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Inscripciones_EventoId_AsistenteId] ON [Inscripciones] ([EventoId], [AsistenteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102052620_AsistentesEInscripciones'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261102052620_AsistentesEInscripciones', N'10.0.14');
END;

COMMIT;
GO

