CREATE TABLE [dbo].[Client]
(
	[ClientId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ClientGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[ClientIsActive] BIT NOT NULL DEFAULT 0, 
	[ClientIsDeleted] BIT NOT NULL DEFAULT 0, 
    [ClientTypeId] INT NOT NULL,
    [ClientName] NVARCHAR(100) NOT NULL, 
    [ClientAddressLine1] NVARCHAR(255) NULL,
    [ClientAddressLine2] NVARCHAR(255) NULL,
    [ClientCity] NVARCHAR(50) NULL,
    [ClientRegion] NVARCHAR(50) NULL,
    [ClientPostalCode] NVARCHAR(10) NULL,
    [ClientCountry] NVARCHAR(50) NULL,
    [ClientPhoneNumber] NVARCHAR(20) NULL,
    [ClientUrl] NVARCHAR(150) NULL, 

    CONSTRAINT [FK_Client_DataDictionary] FOREIGN KEY ([ClientTypeId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),    
    CONSTRAINT [UQ_ClientName] UNIQUE ([ClientName]),
)
