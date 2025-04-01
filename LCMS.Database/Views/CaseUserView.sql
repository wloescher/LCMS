CREATE VIEW CaseUserView AS
	
	SELECT CaseUserId
		, CaseId = CaseUserCaseId
		, UserId = CaseUserUserId
		, CaseTitle
		, UserFullName = UserFirstName + ' ' + UserLastName
		, UserEmailAddress
		, CreatedDate = IsNull(AuditCreated.CaseUserAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.CaseUserAuditDate, IsNull(AuditCreated.CaseUserAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM CaseUser
		LEFT JOIN [Case] ON CaseUserCaseId = CaseId
			AND CaseIsActive = 1
			AND CaseIsDeleted = 0
		LEFT JOIN [User] ON CaseUserUserId = UserId
			AND UserIsActive = 1
			AND UserIsDeleted = 0
		OUTER APPLY 
			(
				SELECT TOP 1 CaseUserAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM CaseUserAudit
					LEFT JOIN [User] ON CaseUserAuditUserId = UserId
				WHERE CaseUserAuditCaseUserId = CaseUserId
					AND CaseUserAuditActionId = 1 -- Created
				ORDER BY CaseUserAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 CaseUserAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM CaseUserAudit
					LEFT JOIN [User] ON CaseUserAuditUserId = UserId
				WHERE CaseUserAuditCaseUserId = CaseUserId
					AND CaseUserAuditActionId = 2 -- Update
				ORDER BY CaseUserAuditDate DESC
			) AS AuditModified
			
	WHERE CaseUserIsDeleted = 0;