CREATE VIEW UserView AS
	
	SELECT UserId
		, [Guid] = UserGuid
		, IsActive = UserIsActive
		, UserAccountId
		, UserAccountIsActive
		, UserName = UserAccountUserName
		, TypeId = UserTypeId
		, [Type] = UserType.DataDictionaryKey
		, FirstName = UserFirstName
		, MiddleName = UserMiddleName
		, LastName = UserLastName
		, FullName = CASE
			WHEN Len(UserMiddleName) = 1 THEN UserFirstName + ' ' + UserMiddleName + '. ' + UserLastName
			WHEN Len(UserMiddleName) > 1 THEN UserFirstName + ' ' + UserMiddleName + ' ' + UserLastName
			ELSE UserFirstName + ' ' + UserLastName
			END
		, EmailAddress = UserEmailAddress
		, AddressLine1 = UserAddressLine1
		, AddressLine2 = UserAddressLine2
		, City = UserCity
		, Region = UserRegion
		, PostalCode = UserPostalCode
		, Country = UserCountry
		, PhoneNumber = UserPhoneNumber
		, CreatedDate = IsNull(AuditCreated.UserAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.UserAuditDate, IsNull(AuditCreated.UserAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM [User]
		LEFT JOIN UserAccount ON UserUserAccountId = UserAccountId
			AND UserAccountIsDeleted = 0
		LEFT JOIN DataDictionary AS UserType ON DataDictionaryGroupId = 2 -- UserType
			AND UserTypeId = UserType.DataDictionaryValue
		OUTER APPLY 
			(
				SELECT TOP 1 UserAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM UserAudit
					LEFT JOIN [User] ON UserAuditUserId = UserId
				WHERE UserAuditUserId = UserId
					AND UserAuditActionId = 1 -- Created
				ORDER BY UserAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 UserAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM UserAudit
					LEFT JOIN [User] ON UserAuditUserId = UserId
				WHERE UserAuditUserId = UserId
					AND UserAuditActionId = 2 -- Update
				ORDER BY UserAuditDate DESC
			) AS AuditModified

	WHERE UserIsDeleted = 0;