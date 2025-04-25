CREATE TABLE [dbo].[VendorDocumentAudit]
(
	[VendorDocumentAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [VendorDocumentAuditActionId] INT NOT NULL, 
    [VendorDocumentAuditVendorDocumentId] INT NOT NULL, 
    [VendorDocumentAuditUserId] INT NOT NULL, 
    [VendorDocumentAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[VendorDocumentAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[VendorDocumentAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[VendorDocumentAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_VendorDocumentAudit_DataDictionary] FOREIGN KEY ([VendorDocumentAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_VendorDocumentAudit_VendorDocument] FOREIGN KEY ([VendorDocumentAuditVendorDocumentId])
		REFERENCES [dbo].[VendorDocument]([VendorDocumentId]),
	CONSTRAINT [FK_VendorDocumentAudit_User] FOREIGN KEY ([VendorDocumentAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
