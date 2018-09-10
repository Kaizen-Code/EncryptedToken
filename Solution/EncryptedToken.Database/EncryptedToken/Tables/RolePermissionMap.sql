CREATE TABLE [EncryptedToken].[RolePermissionMap]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PermissionId] INT NOT NULL, 
    [RoleId] INT NOT NULL
)
