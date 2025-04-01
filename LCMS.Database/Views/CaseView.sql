CREATE VIEW CaseView AS
	
	SELECT CaseId
		, [Guid] = CaseGuid
		, IsActive = CaseIsActive
		, ClientId = CaseClientId
		, ClientName
		, TypeId = CaseTypeId
		, [Type] = CaseType.DataDictionaryKey
		, StatusId = CaseStatusId
		, [Status] = CaseStatus.DataDictionaryKey
		, Title = CaseTitle
		, Summary = CaseSummary
		, CreatedDate = IsNull(AuditCreated.CaseAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.CaseAuditDate, IsNull(AuditCreated.CaseAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM [Case]
		LEFT JOIN Client ON CaseClientId = ClientId
		LEFT JOIN DataDictionary AS CaseType ON CaseType.DataDictionaryGroupId = 4 -- CaseType
			AND CaseTypeId = CaseType.DataDictionaryValue
		LEFT JOIN DataDictionary AS CaseStatus ON CaseStatus.DataDictionaryGroupId = 5 -- CaseStatus
			AND CaseTypeId = CaseStatus.DataDictionaryValue
		OUTER APPLY 
			(
				SELECT TOP 1 CaseAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM CaseAudit
					LEFT JOIN [User] ON CaseAuditUserId = UserId
				WHERE CaseAuditCaseId = CaseId
					AND CaseAuditActionId = 1 -- Created
				ORDER BY CaseAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 CaseAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM CaseAudit
					LEFT JOIN [User] ON CaseAuditUserId = UserId
				WHERE CaseAuditCaseId = CaseId
					AND CaseAuditActionId = 2 -- Update
				ORDER BY CaseAuditDate DESC
			) AS AuditModified

	WHERE CaseIsDeleted = 0;