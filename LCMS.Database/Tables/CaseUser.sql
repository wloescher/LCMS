CREATE TABLE [dbo].[CaseUser]
(
	[CaseUserId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CaseUserCaseId] INT NOT NULL, 
    [CaseUserUserId] INT NOT NULL, 
	[CaseUserIsDeleted] BIT NOT NULL DEFAULT 0, 
	
    CONSTRAINT [FK_CaseUser_Case] FOREIGN KEY ([CaseUserCaseId])
		REFERENCES [dbo].[Case]([CaseId]),
	CONSTRAINT [FK_CaseUser_User] FOREIGN KEY ([CaseUserUserId])
		REFERENCES [dbo].[User]([UserId]),
    CONSTRAINT [UQ_CaseUser] UNIQUE ([CaseUserCaseId], [CaseUserUserId]),
)
