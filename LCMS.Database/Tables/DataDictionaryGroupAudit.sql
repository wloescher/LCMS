CREATE TABLE [dbo].[DataDictionaryGroupAudit]
(
	[DataDictionaryGroupAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DataDictionaryGroupAuditActionId] INT NOT NULL, 
    [DataDictionaryGroupAuditDataDictionaryGroupId] INT NOT NULL, 
    [DataDictionaryGroupAuditUserId] INT NOT NULL, 
    [DataDictionaryGroupAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
	[DataDictionaryGroupAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[DataDictionaryGroupAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[DataDictionaryGroupAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,
    
	CONSTRAINT [FK_DataDictionaryGroupAudit_DataDictionary] FOREIGN KEY ([DataDictionaryGroupAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_DataDictionaryGroupAudit_DataDictionaryGroup] FOREIGN KEY ([DataDictionaryGroupAuditDataDictionaryGroupId])
		REFERENCES [dbo].[DataDictionaryGroup]([DataDictionaryGroupId]),
	CONSTRAINT [FK_DataDictionaryGroupAudit_User] FOREIGN KEY ([DataDictionaryGroupAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
