CREATE VIEW ClientAuditView AS
	
	SELECT ClientAuditId
		, ActionId = ClientAuditActionId
		, [Action] = AuditAction.DataDictionaryKey
		, ClientId = ClientAuditClientId
		, UserId = ClientAuditUserId
		, ClientName
		, UserFullName = UserFirstName + ' ' + UserLastName
		, UserEmailAddress
		, [Date] = ClientAuditDate
		, BeforeJson = ClientAuditBeforeJson
		, AfterJson = ClientAuditAfterJson
		, AffectedColumns = ClientAuditAffectedColumns

	FROM ClientAudit
		LEFT JOIN DataDictionary AS AuditAction ON DataDictionaryGroupId = 1 -- AuditAction
			AND ClientAuditActionId = AuditAction.DataDictionaryValue
		LEFT JOIN Client ON ClientAuditClientId = ClientId
		LEFT JOIN [User] ON ClientAuditUserId = UserId;