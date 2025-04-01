CREATE VIEW DataDictionaryGroupAuditView AS
	
	SELECT DataDictionaryGroupAuditId
		, ActionId = DataDictionaryGroupAuditActionId
		, [Action] = DataDictionaryKey
		, DataDictionaryGroupId = DataDictionaryGroupAuditDataDictionaryGroupId
		, UserId = DataDictionaryGroupAuditUserId
		, DataDictionaryGroupName
		, UserFullName = UserFirstName + ' ' + UserLastName
		, UserEmailAddress
		, [Date] = DataDictionaryGroupAuditDate
		, BeforeJson = DataDictionaryGroupAuditBeforeJson
		, AfterJson = DataDictionaryGroupAuditAfterJson
		, AffectedColumns = DataDictionaryGroupAuditAffectedColumns

	FROM DataDictionaryGroupAudit
		LEFT JOIN DataDictionary AS AuditAction ON DataDictionaryGroupId = 1 -- AuditAction
			AND DataDictionaryGroupAuditActionId = DataDictionaryValue
		LEFT JOIN DataDictionaryGroup ON DataDictionaryGroupAuditDataDictionaryGroupId = DataDictionaryGroup.DataDictionaryGroupId
		LEFT JOIN [User] ON DataDictionaryGroupAuditUserId = UserId;