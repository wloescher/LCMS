CREATE TABLE [dbo].[DataDictionaryAudit]
(
	[DataDictionaryAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DataDictionaryAuditActionId] INT NOT NULL, 
    [DataDictionaryAuditDataDictionaryId] INT NOT NULL, 
    [DataDictionaryAuditUserId] INT NOT NULL, 
    [DataDictionaryAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[DataDictionaryAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[DataDictionaryAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[DataDictionaryAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_DataDictionaryAudit_DataDictionary_ActionId] FOREIGN KEY ([DataDictionaryAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_DataDictionaryAudit_DataDictionary_DataDictionaryId] FOREIGN KEY ([DataDictionaryAuditDataDictionaryId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_DataDictionaryAudit_User] FOREIGN KEY ([DataDictionaryAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
