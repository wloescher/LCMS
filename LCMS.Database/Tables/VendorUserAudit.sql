CREATE TABLE [dbo].[VendorUserAudit]
(
	[VendorUserAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [VendorUserAuditActionId] INT NOT NULL, 
    [VendorUserAuditVendorUserId] INT NOT NULL, 
    [VendorUserAuditUserId] INT NOT NULL, 
    [VendorUserAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[VendorUserAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[VendorUserAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[VendorUserAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_VendorUserAudit_DataDictionary] FOREIGN KEY ([VendorUserAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_VendorUserAudit_VendorUser] FOREIGN KEY ([VendorUserAuditVendorUserId])
		REFERENCES [dbo].[VendorUser]([VendorUserId]),
	CONSTRAINT [FK_VendorUserAudit_User] FOREIGN KEY ([VendorUserAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
