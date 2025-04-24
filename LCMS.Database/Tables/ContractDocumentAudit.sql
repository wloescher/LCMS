CREATE TABLE [dbo].[ContractDocumentAudit]
(
	[ContractDocumentAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractDocumentAuditActionId] INT NOT NULL, 
    [ContractDocumentAuditContractDocumentId] INT NOT NULL, 
    [ContractDocumentAuditUserId] INT NOT NULL, 
    [ContractDocumentAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[ContractDocumentAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[ContractDocumentAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[ContractDocumentAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_ContractDocumentAudit_DataDictionary] FOREIGN KEY ([ContractDocumentAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_ContractDocumentAudit_ContractDocument] FOREIGN KEY ([ContractDocumentAuditContractDocumentId])
		REFERENCES [dbo].[ContractDocument]([ContractDocumentId]),
	CONSTRAINT [FK_ContractDocumentAudit_User] FOREIGN KEY ([ContractDocumentAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
