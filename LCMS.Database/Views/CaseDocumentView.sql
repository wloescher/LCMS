CREATE VIEW CaseDocumentView AS
	
	SELECT CaseDocumentId
		, CaseDocumentGuid
		, CaseId = CaseDocumentCaseId
		, CaseTitle
		, CaseDocumentTypeId
		, DocumentType = DocumentType.DataDictionaryKey
		, DocumentTitle = CaseDocumentTitle
		, DocumentSummary = CaseDocumentSummary
		, OriginalFileName = CaseDocumentOriginalFileName
		, CreatedDate = IsNull(AuditCreated.CaseDocumentAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.CaseDocumentAuditDate, IsNull(AuditCreated.CaseDocumentAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM CaseDocument
		LEFT JOIN [Case] ON CaseDocumentCaseId = CaseId
			AND CaseIsActive = 1
			AND CaseIsDeleted = 0
		LEFT JOIN DataDictionary AS DocumentType ON DocumentType.DataDictionaryGroupId = 6 -- DocumentType
			AND CaseDocumentTypeId = DocumentType.DataDictionaryValue
		OUTER APPLY 
			(
				SELECT TOP 1 CaseDocumentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM CaseDocumentAudit
					LEFT JOIN [User] ON CaseDocumentAuditUserId = UserId
				WHERE CaseDocumentAuditCaseDocumentId = CaseDocumentId
					AND CaseDocumentAuditActionId = 1 -- Created
				ORDER BY CaseDocumentAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 CaseDocumentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM CaseDocumentAudit
					LEFT JOIN [User] ON CaseDocumentAuditUserId = UserId
				WHERE CaseDocumentAuditCaseDocumentId = CaseDocumentId
					AND CaseDocumentAuditActionId = 2 -- Update
				ORDER BY CaseDocumentAuditDate DESC
			) AS AuditModified
			
	WHERE CaseDocumentIsDeleted = 0;