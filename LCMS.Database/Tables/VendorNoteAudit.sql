CREATE TABLE [dbo].[VendorNoteAudit]
(
	[VendorNoteAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [VendorNoteAuditActionId] INT NOT NULL, 
    [VendorNoteAuditVendorNoteId] INT NOT NULL, 
    [VendorNoteAuditUserId] INT NOT NULL, 
    [VendorNoteAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[VendorNoteAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[VendorNoteAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[VendorNoteAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_VendorNoteAudit_DataDictionary] FOREIGN KEY ([VendorNoteAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_VendorNoteAudit_VendorNote] FOREIGN KEY ([VendorNoteAuditVendorNoteId])
		REFERENCES [dbo].[VendorNote]([VendorNoteId]),
	CONSTRAINT [FK_VendorNoteAudit_User] FOREIGN KEY ([VendorNoteAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
