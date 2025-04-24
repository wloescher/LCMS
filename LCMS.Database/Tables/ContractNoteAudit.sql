CREATE TABLE [dbo].[ContractNoteAudit]
(
	[ContractNoteAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractNoteAuditActionId] INT NOT NULL, 
    [ContractNoteAuditContractNoteId] INT NOT NULL, 
    [ContractNoteAuditUserId] INT NOT NULL, 
    [ContractNoteAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[ContractNoteAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[ContractNoteAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[ContractNoteAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_ContractNoteAudit_DataDictionary] FOREIGN KEY ([ContractNoteAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_ContractNoteAudit_ContractNote] FOREIGN KEY ([ContractNoteAuditContractNoteId])
		REFERENCES [dbo].[ContractNote]([ContractNoteId]),
	CONSTRAINT [FK_ContractNoteAudit_User] FOREIGN KEY ([ContractNoteAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
