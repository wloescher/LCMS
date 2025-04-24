CREATE TABLE [dbo].[Contract]
(
	[ContractId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[ContractIsActive] BIT NOT NULL DEFAULT 0, 
	[ContractIsDeleted] BIT NOT NULL DEFAULT 0, 
    [ContractClientId] INT NOT NULL,
    [ContractTypeId] INT NOT NULL,
    [ContractStatusId] INT NOT NULL,
	[ContractTitle] NVARCHAR(150) NULL, 
    [ContractSummary] NVARCHAR(MAX) NULL, 

    CONSTRAINT [FK_Contract_Client] FOREIGN KEY ([ContractClientId])
		REFERENCES [dbo].[Client]([ClientId]),
    CONSTRAINT [FK_Contract_DataDictionary_Type] FOREIGN KEY ([ContractTypeId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
    CONSTRAINT [FK_Contract_DataDictionary_Status] FOREIGN KEY ([ContractStatusId])
		REFERENCES [dbo].[DataDictionary]([DataDictionaryId]),
    CONSTRAINT [UQ_ContractTitle] UNIQUE ([ContractClientId], [ContractTitle]),
)
