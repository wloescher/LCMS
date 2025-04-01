CREATE TABLE [dbo].[CaseCommentAudit]
(
	[CaseCommentAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CaseCommentAuditActionId] INT NOT NULL, 
    [CaseCommentAuditCaseCommentId] INT NOT NULL, 
    [CaseCommentAuditUserId] INT NOT NULL, 
    [CaseCommentAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[CaseCommentAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[CaseCommentAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[CaseCommentAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_CaseCommentAudit_DataDictionary] FOREIGN KEY ([CaseCommentAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_CaseCommentAudit_CaseComment] FOREIGN KEY ([CaseCommentAuditCaseCommentId])
		REFERENCES [dbo].[CaseComment]([CaseCommentId]),
	CONSTRAINT [FK_CaseCommentAudit_User] FOREIGN KEY ([CaseCommentAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
