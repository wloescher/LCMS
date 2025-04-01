CREATE TABLE [dbo].[UserAudit]
(
	[UserAuditId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserAuditActionId] INT NOT NULL, 
    [UserAuditUserId] INT NOT NULL, 
    [UserAuditUserId_Source] INT NOT NULL, 
    [UserAuditDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
	[UserAuditBeforeJson] NVARCHAR(MAX) NOT NULL, 
	[UserAuditAfterJson] NVARCHAR(MAX) NOT NULL,
	[UserAuditAffectedColumns] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT [FK_UserAudit_DataDictionary] FOREIGN KEY ([UserAuditActionId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
	CONSTRAINT [FK_UserAudit_User] FOREIGN KEY ([UserAuditUserId])
		REFERENCES [dbo].[User]([UserId]),
	CONSTRAINT [FK_UserAudit_User_Source] FOREIGN KEY ([UserAuditUserId_Source])
		REFERENCES [dbo].[User]([UserId]),
)
