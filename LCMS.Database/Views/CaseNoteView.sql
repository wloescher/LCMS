CREATE VIEW CaseNoteView AS
	
	SELECT CaseNoteId
		, CaseNoteGuid
		, CaseId = CaseNoteCaseId
		, CaseTitle
		, Note = CaseNoteBody
		, CreatedDate = IsNull(AuditCreated.CaseNoteAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.CaseNoteAuditDate, IsNull(AuditCreated.CaseNoteAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM CaseNote
		LEFT JOIN [Case] ON CaseNoteCaseId = CaseId
			AND CaseIsActive = 1
			AND CaseIsDeleted = 0
		OUTER APPLY 
			(
				SELECT TOP 1 CaseNoteAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM CaseNoteAudit
					LEFT JOIN [User] ON CaseNoteAuditUserId = UserId
				WHERE CaseNoteAuditCaseNoteId = CaseNoteId
					AND CaseNoteAuditActionId = 1 -- Created
				ORDER BY CaseNoteAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 CaseNoteAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM CaseNoteAudit
					LEFT JOIN [User] ON CaseNoteAuditUserId = UserId
				WHERE CaseNoteAuditCaseNoteId = CaseNoteId
					AND CaseNoteAuditActionId = 2 -- Update
				ORDER BY CaseNoteAuditDate DESC
			) AS AuditModified
			
	WHERE CaseNoteIsDeleted = 0;