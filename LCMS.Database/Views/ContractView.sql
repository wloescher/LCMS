CREATE VIEW ContractView AS
	
	SELECT ContractId
		, [Guid] = ContractGuid
		, IsActive = ContractIsActive
		, ClientId = ContractClientId
		, ClientName
		, TypeId = ContractTypeId
		, [Type] = ContractType.DataDictionaryKey
		, StatusId = ContractStatusId
		, [Status] = ContractStatus.DataDictionaryKey
		, Title = ContractTitle
		, Summary = ContractSummary
		, CreatedDate = IsNull(AuditCreated.ContractAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.ContractAuditDate, IsNull(AuditCreated.ContractAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM [Contract]
		LEFT JOIN Client ON ContractClientId = ClientId
		LEFT JOIN DataDictionary AS ContractType ON ContractType.DataDictionaryGroupId = 7 -- ContractType
			AND ContractTypeId = ContractType.DataDictionaryValue
		LEFT JOIN DataDictionary AS ContractStatus ON ContractStatus.DataDictionaryGroupId = 8 -- ContractStatus
			AND ContractTypeId = ContractStatus.DataDictionaryValue
		OUTER APPLY 
			(
				SELECT TOP 1 ContractAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ContractAudit
					LEFT JOIN [User] ON ContractAuditUserId = UserId
				WHERE ContractAuditContractId = ContractId
					AND ContractAuditActionId = 1 -- Created
				ORDER BY ContractAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 ContractAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ContractAudit
					LEFT JOIN [User] ON ContractAuditUserId = UserId
				WHERE ContractAuditContractId = ContractId
					AND ContractAuditActionId = 2 -- Update
				ORDER BY ContractAuditDate DESC
			) AS AuditModified

	WHERE ContractIsDeleted = 0;