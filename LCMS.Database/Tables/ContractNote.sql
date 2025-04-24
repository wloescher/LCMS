CREATE TABLE [dbo].[ContractNote]
(
	[ContractNoteId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractNoteGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[ContractNoteIsDeleted] BIT NOT NULL DEFAULT 0, 
    [ContractNoteContractId] INT NOT NULL, 
    [ContractNoteBody] NVARCHAR(500) NULL,
	
    CONSTRAINT [FK_ContractNote_Contract] FOREIGN KEY ([ContractNoteContractId])
		REFERENCES [dbo].[Contract]([ContractId]),
)
