CREATE TABLE [dbo].[ContractComment]
(
	[ContractCommentId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractCommentGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
	[ContractCommentIsDeleted] BIT NOT NULL DEFAULT 0, 
    [ContractCommentContractId] INT NOT NULL, 
    [ContractCommentBody] NVARCHAR(500) NULL,
	
    CONSTRAINT [FK_ContractComment_Contract] FOREIGN KEY ([ContractCommentContractId])
		REFERENCES [dbo].[Contract]([ContractId]),
)
