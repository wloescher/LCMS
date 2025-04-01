CREATE TABLE [dbo].[CaseComment]
(
	[CaseCommentId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CaseCommentGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[CaseCommentIsDeleted] BIT NOT NULL DEFAULT 0, 
    [CaseCommentCaseId] INT NOT NULL, 
    [CaseCommentBody] NVARCHAR(500) NULL,
	
    CONSTRAINT [FK_CaseComment_Case] FOREIGN KEY ([CaseCommentCaseId])
		REFERENCES [dbo].[Case]([CaseId]),
)
