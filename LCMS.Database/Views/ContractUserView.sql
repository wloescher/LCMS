CREATE VIEW ContractUserView AS
	
	SELECT ContractUserId
		, ContractId = ContractUserContractId
		, UserId = ContractUserUserId
		, ContractTitle
		, UserFullName = UserFirstName + ' ' + UserLastName
		, UserEmailAddress
		, CreatedDate = IsNull(AuditCreated.ContractUserAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.ContractUserAuditDate, IsNull(AuditCreated.ContractUserAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM ContractUser
		LEFT JOIN [Contract] ON ContractUserContractId = ContractId
			AND ContractIsActive = 1
			AND ContractIsDeleted = 0
		LEFT JOIN [User] ON ContractUserUserId = UserId
			AND UserIsActive = 1
			AND UserIsDeleted = 0
		OUTER APPLY 
			(
				SELECT TOP 1 ContractUserAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ContractUserAudit
					LEFT JOIN [User] ON ContractUserAuditUserId = UserId
				WHERE ContractUserAuditContractUserId = ContractUserId
					AND ContractUserAuditActionId = 1 -- Created
				ORDER BY ContractUserAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 ContractUserAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ContractUserAudit
					LEFT JOIN [User] ON ContractUserAuditUserId = UserId
				WHERE ContractUserAuditContractUserId = ContractUserId
					AND ContractUserAuditActionId = 2 -- Update
				ORDER BY ContractUserAuditDate DESC
			) AS AuditModified
			
	WHERE ContractUserIsDeleted = 0;