CREATE VIEW DataDictionaryAuditView AS
	
	SELECT DataDictionaryAuditId
		, ActionId = DataDictionaryAuditActionId
		, [Action] = AuditAction.DataDictionaryKey
		, DataDictionaryId = DataDictionaryAuditDataDictionaryId
		, UserId = DataDictionaryAuditUserId
		, Source.DataDictionaryKey
		, UserFullName = UserFirstName + ' ' + UserLastName
		, UserEmailAddress
		, [Date] = DataDictionaryAuditDate
		, BeforeJson = DataDictionaryAuditBeforeJson
		, AfterJson = DataDictionaryAuditAfterJson
		, AffectedColumns = DataDictionaryAuditAffectedColumns

	FROM DataDictionaryAudit
		LEFT JOIN DataDictionary AS AuditAction ON DataDictionaryGroupId = 1 -- AuditAction
			AND DataDictionaryAuditActionId = AuditAction.DataDictionaryValue
		LEFT JOIN DataDictionary AS Source ON DataDictionaryAuditDataDictionaryId = Source.DataDictionaryId
		LEFT JOIN [User] ON DataDictionaryAuditUserId = UserId;