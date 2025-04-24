CREATE TABLE [dbo].[ContractCommentAudit]
(
	[ContractCommentAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractCommentAuditActionId] INT NOT NULL, 
    [ContractCommentAuditContractCommentId] INT NOT NULL, 
    [ContractCommentAuditUserId] INT NOT NULL, 
    [ContractCommentAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[ContractCommentAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[ContractCommentAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[ContractCommentAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_ContractCommentAudit_DataDictionary] FOREIGN KEY ([ContractCommentAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_ContractCommentAudit_ContractComment] FOREIGN KEY ([ContractCommentAuditContractCommentId])
		REFERENCES [dbo].[ContractComment]([ContractCommentId]),
	CONSTRAINT [FK_ContractCommentAudit_User] FOREIGN KEY ([ContractCommentAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
