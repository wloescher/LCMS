CREATE TABLE [dbo].[DataDictionary]
(
	[DataDictionaryId] INT NOT NULL PRIMARY KEY IDENTITY, 
	[DataDictionaryIsActive] BIT NOT NULL DEFAULT 0, 
	[DataDictionaryIsDeleted] BIT NOT NULL DEFAULT 0, 
    [DataDictionaryGroupId] INT NOT NULL, 
    [DataDictionaryKey] NVARCHAR(50) NOT NULL, 
    [DataDictionaryValue] INT NOT NULL,
    [DataDictionaryDescription] NVARCHAR(150) NULL,

	CONSTRAINT [FK_DataDictionary_DataDictionarGroup] FOREIGN KEY ([DataDictionaryGroupId])
		REFERENCES [dbo].[DataDictionaryGroup]([DataDictionaryGroupId]),
    CONSTRAINT [UQ_DataDictionaryKey] UNIQUE ([DataDictionaryGroupId],[DataDictionaryKey]),
)
