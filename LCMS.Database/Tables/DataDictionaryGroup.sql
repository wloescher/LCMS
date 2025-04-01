CREATE TABLE [dbo].[DataDictionaryGroup]
(
	[DataDictionaryGroupId] INT NOT NULL PRIMARY KEY IDENTITY, 
	[DataDictionaryGroupIsActive] BIT NOT NULL DEFAULT 0, 
	[DataDictionaryGroupIsDeleted] BIT NOT NULL DEFAULT 0, 
    [DataDictionaryGroupName] NVARCHAR(50) NOT NULL,

    CONSTRAINT [UQ_DataDictionaryGroupName] UNIQUE ([DataDictionaryGroupName]),
)