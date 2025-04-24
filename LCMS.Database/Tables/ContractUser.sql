CREATE TABLE [dbo].[ContractUser]
(
	[ContractUserId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractUserContractId] INT NOT NULL, 
    [ContractUserUserId] INT NOT NULL, 
	[ContractUserIsDeleted] BIT NOT NULL DEFAULT 0, 
	
    CONSTRAINT [FK_ContractUser_Contract] FOREIGN KEY ([ContractUserContractId])
		REFERENCES [dbo].[Contract]([ContractId]),
	CONSTRAINT [FK_ContractUser_User] FOREIGN KEY ([ContractUserUserId])
		REFERENCES [dbo].[User]([UserId]),
    CONSTRAINT [UQ_ContractUser] UNIQUE ([ContractUserContractId], [ContractUserUserId]),
)
