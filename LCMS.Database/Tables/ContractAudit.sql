CREATE TABLE [dbo].[ContractAudit]
(
	[ContractAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractAuditActionId] INT NOT NULL, 
    [ContractAuditContractId] INT NOT NULL, 
    [ContractAuditUserId] INT NOT NULL, 
    [ContractAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[ContractAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[ContractAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[ContractAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_ContractAudit_DataDictionary] FOREIGN KEY ([ContractAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_ContractAudit_Contract] FOREIGN KEY ([ContractAuditContractId])
		REFERENCES [dbo].[Contract]([ContractId]),
	CONSTRAINT [FK_ContractAudit_User] FOREIGN KEY ([ContractAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
