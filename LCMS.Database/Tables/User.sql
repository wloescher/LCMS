CREATE TABLE [dbo].[User]
(
	[UserId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[UserIsActive] BIT NOT NULL DEFAULT 0, 
	[UserIsDeleted] BIT NOT NULL DEFAULT 0, 
    [UserUserAccountId] INT NULL,
    [UserTypeId] INT NOT NULL,
    [UserFirstName] NVARCHAR(50) NULL, 
    [UserMiddleName] NVARCHAR(50) NULL, 
    [UserLastName] NVARCHAR(50) NULL, 
    [UserEmailAddress] NVARCHAR(100) NOT NULL, 
    [UserAddressLine1] NVARCHAR(255) NULL,
    [UserAddressLine2] NVARCHAR(255) NULL,
    [UserCity] NVARCHAR(50) NULL,
    [UserRegion] NVARCHAR(50) NULL,
    [UserPostalCode] NVARCHAR(10) NULL,
    [UserCountry] NVARCHAR(50) NULL,
    [UserPhoneNumber] NVARCHAR(20) NULL,

	CONSTRAINT [FK_User_UserAccount] FOREIGN KEY ([UserUserAccountId])
		REFERENCES [dbo].[UserAccount]([UserAccountId]),
    CONSTRAINT [FK_User_DataDictionary] FOREIGN KEY ([UserTypeId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
    CONSTRAINT [UQ_UserEmailAddress] UNIQUE ([UserEmailAddress]),
)
