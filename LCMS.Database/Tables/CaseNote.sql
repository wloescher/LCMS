CREATE TABLE [dbo].[CaseNote]
(
	[CaseNoteId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CaseNoteGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[CaseNoteIsDeleted] BIT NOT NULL DEFAULT 0, 
    [CaseNoteCaseId] INT NOT NULL, 
    [CaseNoteBody] NVARCHAR(500) NULL,
	
    CONSTRAINT [FK_CaseNote_Case] FOREIGN KEY ([CaseNoteCaseId])
		REFERENCES [dbo].[Case]([CaseId]),
)
