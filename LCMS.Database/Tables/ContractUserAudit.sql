CREATE TABLE [dbo].[ContractUserAudit]
(
	[ContractUserAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractUserAuditActionId] INT NOT NULL, 
    [ContractUserAuditContractUserId] INT NOT NULL, 
    [ContractUserAuditUserId] INT NOT NULL, 
    [ContractUserAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[ContractUserAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[ContractUserAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[ContractUserAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_ContractUserAudit_DataDictionary] FOREIGN KEY ([ContractUserAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_ContractUserAudit_ContractUser] FOREIGN KEY ([ContractUserAuditContractUserId])
		REFERENCES [dbo].[ContractUser]([ContractUserId]),
	CONSTRAINT [FK_ContractUserAudit_User] FOREIGN KEY ([ContractUserAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
