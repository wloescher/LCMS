CREATE TABLE [dbo].[CaseDocumentAudit]
(
	[CaseDocumentAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CaseDocumentAuditActionId] INT NOT NULL, 
    [CaseDocumentAuditCaseDocumentId] INT NOT NULL, 
    [CaseDocumentAuditUserId] INT NOT NULL, 
    [CaseDocumentAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[CaseDocumentAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[CaseDocumentAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[CaseDocumentAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_CaseDocumentAudit_DataDictionary] FOREIGN KEY ([CaseDocumentAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_CaseDocumentAudit_CaseDocument] FOREIGN KEY ([CaseDocumentAuditCaseDocumentId])
		REFERENCES [dbo].[CaseDocument]([CaseDocumentId]),
	CONSTRAINT [FK_CaseDocumentAudit_User] FOREIGN KEY ([CaseDocumentAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
