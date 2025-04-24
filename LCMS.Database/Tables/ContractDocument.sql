CREATE TABLE [dbo].[ContractDocument]
(
	[ContractDocumentId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractDocumentGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[ContractDocumentIsDeleted] BIT NOT NULL DEFAULT 0, 
    [ContractDocumentContractId] INT NOT NULL, 
    [ContractDocumentTypeId] INT NOT NULL,
	[ContractDocumentTitle] NVARCHAR(255),
    [ContractDocumentSummary] NVARCHAR(500) NULL,
	[ContractDocumentOriginalFileName] NVARCHAR(255),
	
    CONSTRAINT [FK_ContractDocument_Contract] FOREIGN KEY ([ContractDocumentContractId])
		REFERENCES [dbo].[Contract]([ContractId]),
)
