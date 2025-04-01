CREATE TABLE [dbo].[ClientAudit]
(
	[ClientAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ClientAuditActionId] INT NOT NULL, 
    [ClientAuditClientId] INT NOT NULL, 
    [ClientAuditUserId] INT NOT NULL, 
    [ClientAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[ClientAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[ClientAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[ClientAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_ClientAudit_DataDictionary] FOREIGN KEY ([ClientAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_ClientAudit_Client] FOREIGN KEY ([ClientAuditClientId])
		REFERENCES [dbo].[Client]([ClientId]),
	CONSTRAINT [FK_ClientAudit_User] FOREIGN KEY ([ClientAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
