CREATE TABLE [dbo].[UserAccountAudit]
(
	[UserAccountAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserAccountAuditActionId] INT NOT NULL, 
    [UserAccountAuditUserAccountId] INT NOT NULL, 
    [UserAccountAuditUserId] INT NOT NULL, 
    [UserAccountAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[UserAccountAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[UserAccountAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[UserAccountAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_UserAccountAudit_DataDictionary] FOREIGN KEY ([UserAccountAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_UserAccountAudit_UserAccount] FOREIGN KEY ([UserAccountAuditUserAccountId])
		REFERENCES [dbo].[UserAccount]([UserAccountId]),
	CONSTRAINT [FK_UserAccountAudit_User] FOREIGN KEY ([UserAccountAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
)
