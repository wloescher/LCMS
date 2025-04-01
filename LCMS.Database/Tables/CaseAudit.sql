CREATE TABLE [dbo].[CaseAudit]
(
	[CaseAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CaseAuditActionId] INT NOT NULL, 
    [CaseAuditCaseId] INT NOT NULL, 
    [CaseAuditUserId] INT NOT NULL, 
    [CaseAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[CaseAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[CaseAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[CaseAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_CaseAudit_DataDictionary] FOREIGN KEY ([CaseAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_CaseAudit_Case] FOREIGN KEY ([CaseAuditCaseId])
		REFERENCES [dbo].[Case]([CaseId]),
	CONSTRAINT [FK_CaseAudit_User] FOREIGN KEY ([CaseAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
