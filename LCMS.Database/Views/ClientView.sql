CREATE VIEW ClientView AS
	
	SELECT ClientId
		, [Guid] = ClientGuid
		, IsActive = ClientIsActive
		, TypeId = ClientTypeId
		, [Type] = ClientType.DataDictionaryKey
		, [Name] = ClientName
		, AddressLine1 = ClientAddressLine1
		, AddressLine2 = ClientAddressLine2
		, City = ClientCity
		, Region = ClientRegion
		, PostalCode = ClientPostalCode
		, Country = ClientCountry
		, PhoneNumber = ClientPhoneNumber
		, [Url] = ClientUrl
		, CreatedDate = IsNull(AuditCreated.ClientAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.ClientAuditDate, IsNull(AuditCreated.ClientAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM Client
		LEFT JOIN DataDictionary AS ClientType ON DataDictionaryGroupId = 3 -- ClientType
			AND ClientTypeId = ClientType.DataDictionaryValue
		OUTER APPLY 
			(
				SELECT TOP 1 ClientAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ClientAudit
					LEFT JOIN [User] ON ClientAuditUserId = UserId
				WHERE ClientAuditClientId = ClientId
					AND ClientAuditActionId = 1 -- Created
				ORDER BY ClientAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 ClientAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ClientAudit
					LEFT JOIN [User] ON ClientAuditUserId = UserId
				WHERE ClientAuditClientId = ClientId
					AND ClientAuditActionId = 2 -- Update
				ORDER BY ClientAuditDate DESC
			) AS AuditModified

	WHERE ClientIsDeleted = 0;