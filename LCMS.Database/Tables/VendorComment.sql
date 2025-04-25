CREATE TABLE [dbo].[VendorComment]
(
	[VendorCommentId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [VendorCommentGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[VendorCommentIsDeleted] BIT NOT NULL DEFAULT 0, 
    [VendorCommentVendorId] INT NOT NULL, 
    [VendorCommentBody] NVARCHAR(500) NULL,
	
    CONSTRAINT [FK_VendorComment_Vendor] FOREIGN KEY ([VendorCommentVendorId])
		REFERENCES [dbo].[Vendor]([VendorId]),
)
