CREATE VIEW ContractDocumentView AS
	
	SELECT ContractDocumentId
		, ContractDocumentGuid
		, ContractId = ContractDocumentContractId
		, ContractTitle
		, ContractDocumentTypeId
		, DocumentType = DocumentType.DataDictionaryKey
		, DocumentTitle = ContractDocumentTitle
		, DocumentSummary = ContractDocumentSummary
		, OriginalFileName = ContractDocumentOriginalFileName
		, CreatedDate = IsNull(AuditCreated.ContractDocumentAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.ContractDocumentAuditDate, IsNull(AuditCreated.ContractDocumentAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM ContractDocument
		LEFT JOIN [Contract] ON ContractDocumentContractId = ContractId
			AND ContractIsActive = 1
			AND ContractIsDeleted = 0
		LEFT JOIN DataDictionary AS DocumentType ON DocumentType.DataDictionaryGroupId = 6 -- DocumentType
			AND ContractDocumentTypeId = DocumentType.DataDictionaryValue
		OUTER APPLY 
			(
				SELECT TOP 1 ContractDocumentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ContractDocumentAudit
					LEFT JOIN [User] ON ContractDocumentAuditUserId = UserId
				WHERE ContractDocumentAuditContractDocumentId = ContractDocumentId
					AND ContractDocumentAuditActionId = 1 -- Created
				ORDER BY ContractDocumentAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 ContractDocumentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ContractDocumentAudit
					LEFT JOIN [User] ON ContractDocumentAuditUserId = UserId
				WHERE ContractDocumentAuditContractDocumentId = ContractDocumentId
					AND ContractDocumentAuditActionId = 2 -- Update
				ORDER BY ContractDocumentAuditDate DESC
			) AS AuditModified
			
	WHERE ContractDocumentIsDeleted = 0;