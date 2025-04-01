CREATE VIEW CaseAuditView AS
	
	SELECT CaseAuditId
		, ActionId = CaseAuditActionId
		, [Action] = AuditAction.DataDictionaryKey
		, CaseId = CaseAuditCaseId
		, UserId = CaseAuditUserId
		, CaseTitle
		, UserFullName = UserFirstName + ' ' + UserLastName
		, UserEmailAddress
		, [Date] = CaseAuditDate
		, BeforeJson = CaseAuditBeforeJson
		, AfterJson = CaseAuditAfterJson
		, AffectedColumns = CaseAuditAffectedColumns

	FROM CaseAudit
		LEFT JOIN DataDictionary AS AuditAction ON DataDictionaryGroupId = 1 -- AuditAction
			AND CaseAuditActionId = AuditAction.DataDictionaryValue
		LEFT JOIN [Case] ON CaseAuditCaseId = CaseId
		LEFT JOIN [User] ON CaseAuditUserId = UserId;