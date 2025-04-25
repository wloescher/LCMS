CREATE TABLE [dbo].[VendorUser]
(
	[VendorUserId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [VendorUserVendorId] INT NOT NULL, 
    [VendorUserUserId] INT NOT NULL, 
	[VendorUserIsDeleted] BIT NOT NULL DEFAULT 0, 
	
    CONSTRAINT [FK_VendorUser_Vendor] FOREIGN KEY ([VendorUserVendorId])
		REFERENCES [dbo].[Vendor]([VendorId]),
	CONSTRAINT [FK_VendorUser_User] FOREIGN KEY ([VendorUserUserId])
		REFERENCES [dbo].[User]([UserId]),
    CONSTRAINT [UQ_VendorUser] UNIQUE ([VendorUserVendorId], [VendorUserUserId]),
)
