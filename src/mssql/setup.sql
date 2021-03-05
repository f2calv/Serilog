IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'Serilog')
    BEGIN
        CREATE DATABASE [Serilog]
    END
ELSE
    BEGIN
        PRINT 'Serilog database already exists, skipping creation...'
    END

-- CREATE TABLE [Logs]
-- (
--    [Id] int IDENTITY(1,1) NOT NULL,
--    [Message] nvarchar(max) NULL,
--    [MessageTemplate] nvarchar(max) NULL,
--    [Level] nvarchar(128) NULL,
--    [TimeStamp] datetime NOT NULL,
--    [Exception] nvarchar(max) NULL,
--    [Properties] nvarchar(max) NULL
--    CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED ([Id] ASC) 
-- );
-- GO

-- CREATE DATABASE TodoDb;
-- GO

-- USE TodoDb;
-- GO

-- CREATE TABLE TodoItems
-- (
--     Id INT IDENTITY(1,1) PRIMARY KEY,
--     Description NVARCHAR(MAX) NOT NULL,
--     IsCompleted BIT DEFAULT(0),
--     CreatedOn DATETIME DEFAULT(GETUTCDATE())
-- )