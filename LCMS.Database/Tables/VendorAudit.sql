CREATE TABLE [dbo].[VendorAudit]
(
	[VendorAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [VendorAuditActionId] INT NOT NULL, 
    [VendorAuditVendorId] INT NOT NULL, 
    [VendorAuditUserId] INT NOT NULL, 
    [VendorAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[VendorAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[VendorAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[VendorAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_VendorAudit_DataDictionary] FOREIGN KEY ([VendorAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_VendorAudit_Vendor] FOREIGN KEY ([VendorAuditVendorId])
		REFERENCES [dbo].[Vendor]([VendorId]),
	CONSTRAINT [FK_VendorAudit_User] FOREIGN KEY ([VendorAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
