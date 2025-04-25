CREATE TABLE [dbo].[VendorCommentAudit]
(
	[VendorCommentAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [VendorCommentAuditActionId] INT NOT NULL, 
    [VendorCommentAuditVendorCommentId] INT NOT NULL, 
    [VendorCommentAuditUserId] INT NOT NULL, 
    [VendorCommentAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[VendorCommentAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[VendorCommentAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[VendorCommentAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_VendorCommentAudit_DataDictionary] FOREIGN KEY ([VendorCommentAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_VendorCommentAudit_VendorComment] FOREIGN KEY ([VendorCommentAuditVendorCommentId])
		REFERENCES [dbo].[VendorComment]([VendorCommentId]),
	CONSTRAINT [FK_VendorCommentAudit_User] FOREIGN KEY ([VendorCommentAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
