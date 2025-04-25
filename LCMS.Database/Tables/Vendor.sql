CREATE TABLE [dbo].[Vendor]
(
	[VendorId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [VendorGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[VendorIsActive] BIT NOT NULL DEFAULT 0, 
	[VendorIsDeleted] BIT NOT NULL DEFAULT 0, 
    [VendorTypeId] INT NOT NULL,
    [VendorStatusId] INT NOT NULL,
    [VendorName] NVARCHAR(100) NOT NULL, 
    [VendorAddressLine1] NVARCHAR(255) NULL,
    [VendorAddressLine2] NVARCHAR(255) NULL,
    [VendorCity] NVARCHAR(50) NULL,
    [VendorRegion] NVARCHAR(50) NULL,
    [VendorPostalCode] NVARCHAR(10) NULL,
    [VendorCountry] NVARCHAR(50) NULL,
    [VendorPhoneNumber] NVARCHAR(20) NULL,
    [VendorUrl] NVARCHAR(150) NULL, 

    CONSTRAINT [FK_Vendor_DataDictionary_Type] FOREIGN KEY ([VendorTypeId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),    
    CONSTRAINT [FK_Vendor_DataDictionary_Status] FOREIGN KEY ([VendorStatusId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),    
    CONSTRAINT [UQ_VendorName] UNIQUE ([VendorName]),
)
