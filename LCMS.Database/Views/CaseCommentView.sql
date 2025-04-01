CREATE VIEW CaseCommentView AS
	
	SELECT CaseCommentId
		, CaseCommentGuid
		, CaseId = CaseCommentCaseId
		, CaseTitle
		, Comment = CaseCommentBody
		, CreatedDate = IsNull(AuditCreated.CaseCommentAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.CaseCommentAuditDate, IsNull(AuditCreated.CaseCommentAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))
		, IsNull(AuditCreated.CaseCommentAuditDate, '1/1/1900') AS CreatedDate2

	FROM CaseComment
		LEFT JOIN [Case] ON CaseCommentCaseId = CaseId
			AND CaseIsActive = 1
			AND CaseIsDeleted = 0
		OUTER APPLY 
			(
				SELECT TOP 1 CaseCommentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM CaseCommentAudit
					LEFT JOIN [User] ON CaseCommentAuditUserId = UserId
				WHERE CaseCommentAuditCaseCommentId = CaseCommentId
					AND CaseCommentAuditActionId = 1 -- Created
				ORDER BY CaseCommentAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 CaseCommentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM CaseCommentAudit
					LEFT JOIN [User] ON CaseCommentAuditUserId = UserId
				WHERE CaseCommentAuditCaseCommentId = CaseCommentId
					AND CaseCommentAuditActionId = 2 -- Update
				ORDER BY CaseCommentAuditDate DESC
			) AS AuditModified
			
	WHERE CaseCommentIsDeleted = 0;