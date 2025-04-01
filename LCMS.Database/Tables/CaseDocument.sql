CREATE TABLE [dbo].[CaseDocument]
(
	[CaseDocumentId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CaseDocumentGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[CaseDocumentIsDeleted] BIT NOT NULL DEFAULT 0, 
    [CaseDocumentCaseId] INT NOT NULL, 
    [CaseDocumentTypeId] INT NOT NULL,
	[CaseDocumentTitle] NVARCHAR(255),
    [CaseDocumentSummary] NVARCHAR(500) NULL,
	[CaseDocumentOriginalFileName] NVARCHAR(255),
	
    CONSTRAINT [FK_CaseDocument_Case] FOREIGN KEY ([CaseDocumentCaseId])
		REFERENCES [dbo].[Case]([CaseId]),
)
