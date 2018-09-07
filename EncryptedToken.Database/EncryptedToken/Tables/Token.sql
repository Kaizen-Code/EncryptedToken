CREATE TABLE [EncryptedToken].[Token]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserName] VARCHAR(100) NOT NULL, 
    [Roles] VARCHAR(700) NULL, 
    [CreatedDateTime] DATETIME NOT NULL, 
    [ExpiryDateTime] DATETIME NOT NULL, 
    [LogoutDateTime] DATETIME NULL, 
    [EncryptedValue] VARCHAR(1000) NULL
)
