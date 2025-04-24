CREATE VIEW ContractCommentView AS
	
	SELECT ContractCommentId
		, ContractCommentGuid
		, ContractId = ContractCommentContractId
		, ContractTitle
		, Comment = ContractCommentBody
		, CreatedDate = IsNull(AuditCreated.ContractCommentAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.ContractCommentAuditDate, IsNull(AuditCreated.ContractCommentAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))
		, IsNull(AuditCreated.ContractCommentAuditDate, '1/1/1900') AS CreatedDate2

	FROM ContractComment
		LEFT JOIN [Contract] ON ContractCommentContractId = ContractId
			AND ContractIsActive = 1
			AND ContractIsDeleted = 0
		OUTER APPLY 
			(
				SELECT TOP 1 ContractCommentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ContractCommentAudit
					LEFT JOIN [User] ON ContractCommentAuditUserId = UserId
				WHERE ContractCommentAuditContractCommentId = ContractCommentId
					AND ContractCommentAuditActionId = 1 -- Created
				ORDER BY ContractCommentAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 ContractCommentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ContractCommentAudit
					LEFT JOIN [User] ON ContractCommentAuditUserId = UserId
				WHERE ContractCommentAuditContractCommentId = ContractCommentId
					AND ContractCommentAuditActionId = 2 -- Update
				ORDER BY ContractCommentAuditDate DESC
			) AS AuditModified
			
	WHERE ContractCommentIsDeleted = 0;