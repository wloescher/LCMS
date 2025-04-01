CREATE TABLE [dbo].[CaseNoteAudit]
(
	[CaseNoteAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CaseNoteAuditActionId] INT NOT NULL, 
    [CaseNoteAuditCaseNoteId] INT NOT NULL, 
    [CaseNoteAuditUserId] INT NOT NULL, 
    [CaseNoteAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[CaseNoteAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[CaseNoteAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[CaseNoteAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_CaseNoteAudit_DataDictionary] FOREIGN KEY ([CaseNoteAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_CaseNoteAudit_CaseNote] FOREIGN KEY ([CaseNoteAuditCaseNoteId])
		REFERENCES [dbo].[CaseNote]([CaseNoteId]),
	CONSTRAINT [FK_CaseNoteAudit_User] FOREIGN KEY ([CaseNoteAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
