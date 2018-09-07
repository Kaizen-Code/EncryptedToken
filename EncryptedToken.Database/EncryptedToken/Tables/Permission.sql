CREATE TABLE [EncryptedToken].[Permission]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(100) NOT NULL, 
    [Caption] NVARCHAR(100) NOT NULL, 
    [Description] NVARCHAR(200) NULL
)
