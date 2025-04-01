CREATE TABLE [dbo].[Case]
(
	[CaseId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CaseGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[CaseIsActive] BIT NOT NULL DEFAULT 0, 
	[CaseIsDeleted] BIT NOT NULL DEFAULT 0, 
    [CaseClientId] INT NOT NULL,
    [CaseTypeId] INT NOT NULL,
    [CaseStatusId] INT NOT NULL,
	[CaseTitle] NVARCHAR(150) NULL, 
    [CaseSummary] NVARCHAR(MAX) NULL, 

    CONSTRAINT [FK_Case_Client] FOREIGN KEY ([CaseClientId])
		REFERENCES [dbo].[Client]([ClientId]),
    CONSTRAINT [FK_Case_DataDictionary_Type] FOREIGN KEY ([CaseTypeId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
    CONSTRAINT [FK_Case_DataDictionary_Status] FOREIGN KEY ([CaseStatusId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
    CONSTRAINT [UQ_CaseTitle] UNIQUE ([CaseClientId], [CaseTitle]),
)
