CREATE VIEW UserAccountView AS
	
	SELECT UserAccountId
		, [Guid] = UserAccountGuid
		, IsActive = UserAccountIsActive
		, IsDeleted = UserAccountIsDeleted
		, UserName = UserAccountUserName
		, [Password] = UserAccountPassword
		, PasswordHash = UserAccountPasswordHash
		, PasswordAttemptCount = UserAccountPasswordAttemptCount
		, CreatedDate = IsNull(AuditCreated.UserAccountAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.UserAccountAuditDate, IsNull(AuditCreated.UserAccountAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM UserAccount
		OUTER APPLY 
			(
				SELECT TOP 1 UserAccountAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM UserAccountAudit
					LEFT JOIN [User] ON UserAccountAuditUserId = UserId
				WHERE UserAccountAuditUserAccountId = UserAccountId
					AND UserAccountAuditActionId = 1 -- Created
				ORDER BY UserAccountAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 UserAccountAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM UserAccountAudit
					LEFT JOIN [User] ON UserAccountAuditUserId = UserId
				WHERE UserAccountAuditUserAccountId = UserAccountId
					AND UserAccountAuditActionId = 2 -- Update
				ORDER BY UserAccountAuditDate DESC
			) AS AuditModified

	WHERE UserAccountIsDeleted = 0;