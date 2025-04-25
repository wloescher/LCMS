CREATE TABLE [dbo].[VendorDocument]
(
	[VendorDocumentId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [VendorDocumentGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[VendorDocumentIsDeleted] BIT NOT NULL DEFAULT 0, 
    [VendorDocumentVendorId] INT NOT NULL, 
    [VendorDocumentTypeId] INT NOT NULL,
	[VendorDocumentTitle] NVARCHAR(255),
    [VendorDocumentSummary] NVARCHAR(500) NULL,
	[VendorDocumentOriginalFileName] NVARCHAR(255),
	
    CONSTRAINT [FK_VendorDocument_Vendor] FOREIGN KEY ([VendorDocumentVendorId])
		REFERENCES [dbo].[Vendor]([VendorId]),
)
