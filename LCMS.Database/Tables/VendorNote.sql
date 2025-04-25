CREATE TABLE [dbo].[VendorNote]
(
	[VendorNoteId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [VendorNoteGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[VendorNoteIsDeleted] BIT NOT NULL DEFAULT 0, 
    [VendorNoteVendorId] INT NOT NULL, 
    [VendorNoteBody] NVARCHAR(500) NULL,
	
    CONSTRAINT [FK_VendorNote_Vendor] FOREIGN KEY ([VendorNoteVendorId])
		REFERENCES [dbo].[Vendor]([VendorId]),
)
