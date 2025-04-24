CREATE VIEW ContractAuditView AS
	
	SELECT ContractAuditId
		, ActionId = ContractAuditActionId
		, [Action] = AuditAction.DataDictionaryKey
		, ContractId = ContractAuditContractId
		, UserId = ContractAuditUserId
		, ContractTitle
		, UserFullName = UserFirstName + ' ' + UserLastName
		, UserEmailAddress
		, [Date] = ContractAuditDate
		, BeforeJson = ContractAuditBeforeJson
		, AfterJson = ContractAuditAfterJson
		, AffectedColumns = ContractAuditAffectedColumns

	FROM ContractAudit
		LEFT JOIN DataDictionary AS AuditAction ON DataDictionaryGroupId = 1 -- AuditAction
			AND ContractAuditActionId = AuditAction.DataDictionaryValue
		LEFT JOIN [Contract] ON ContractAuditContractId = ContractId
		LEFT JOIN [User] ON ContractAuditUserId = UserId;