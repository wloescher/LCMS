CREATE TABLE [dbo].[CaseUserAudit]
(
	[CaseUserAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CaseUserAuditActionId] INT NOT NULL, 
    [CaseUserAuditCaseUserId] INT NOT NULL, 
    [CaseUserAuditUserId] INT NOT NULL, 
    [CaseUserAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[CaseUserAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[CaseUserAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[CaseUserAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_CaseUserAudit_DataDictionary] FOREIGN KEY ([CaseUserAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_CaseUserAudit_CaseUser] FOREIGN KEY ([CaseUserAuditCaseUserId])
		REFERENCES [dbo].[CaseUser]([CaseUserId]),
	CONSTRAINT [FK_CaseUserAudit_User] FOREIGN KEY ([CaseUserAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
