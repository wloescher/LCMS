CREATE TABLE [dbo].[UserAccount]
(
	[UserAccountId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserAccountGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[UserAccountIsActive] BIT NOT NULL DEFAULT 0, 
	[UserAccountIsDeleted] BIT NOT NULL DEFAULT 0,
    [UserAccountUserName] NVARCHAR(50) NOT NULL, 
    [UserAccountPassword] NVARCHAR(50) NOT NULL,
    [UserAccountPasswordHash] NVARCHAR(255) NOT NULL,
    [UserAccountPasswordAttemptCount] INT NOT NULL DEFAULT -1,

    CONSTRAINT [UQ_UserAccountUserName] UNIQUE ([UserAccountUserName]),
)
