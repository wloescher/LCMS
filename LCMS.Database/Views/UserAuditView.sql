CREATE VIEW UserAuditView AS
	
	SELECT UserAuditId
		, ActionId = UserAuditActionId
		, [Action] = AuditAction.DataDictionaryKey
		, UserId = UserAuditUserId
		, UserFullName = UserTarget.UserFirstName + ' ' + UserTarget.UserLastName
		, UserEmailAddress = UserTarget.UserEmailAddress
		, UserId_Source = UserAuditUserId_Source
		, UserFullName_Source = UserSource.UserFirstName + ' ' + UserSource.UserLastName
		, UserEmailAddress_Source = UserSource.UserEmailAddress
		, [Date] = UserAuditDate
		, BeforeJson = UserAuditBeforeJson
		, AfterJson = UserAuditAfterJson
		, AffectedColumns = UserAuditAffectedColumns

	FROM UserAudit
		LEFT JOIN DataDictionary AS AuditAction ON DataDictionaryGroupId = 1 -- AuditAction
			AND UserAuditActionId = AuditAction.DataDictionaryValue
		LEFT JOIN [User] AS UserTarget ON UserAuditUserId = UserTarget.UserId
		LEFT JOIN [User] AS UserSource ON UserAuditUserId_Source = UserSource.UserId;