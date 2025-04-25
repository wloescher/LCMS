CREATE VIEW VendorView AS
	
	SELECT VendorId
		, [Guid] = VendorGuid
		, IsActive = VendorIsActive
		, TypeId = VendorTypeId
		, StatusId = VendorStatusId
		, [Type] = VendorType.DataDictionaryKey
		, [Status] = VendorStatus.DataDictionaryKey
		, [Name] = VendorName
		, AddressLine1 = VendorAddressLine1
		, AddressLine2 = VendorAddressLine2
		, City = VendorCity
		, Region = VendorRegion
		, PostalCode = VendorPostalCode
		, Country = VendorCountry
		, PhoneNumber = VendorPhoneNumber
		, [Url] = VendorUrl
		, CreatedDate = IsNull(AuditCreated.VendorAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.VendorAuditDate, IsNull(AuditCreated.VendorAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM Vendor
		LEFT JOIN DataDictionary AS VendorType ON VendorType.DataDictionaryGroupId = 9 -- VendorType
			AND VendorTypeId = VendorType.DataDictionaryValue
		LEFT JOIN DataDictionary AS VendorStatus ON VendorStatus.DataDictionaryGroupId = 10 -- VendorStatus
			AND VendorStatusId = VendorStatus.DataDictionaryValue
		OUTER APPLY 
			(
				SELECT TOP 1 VendorAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM VendorAudit
					LEFT JOIN [User] ON VendorAuditUserId = UserId
				WHERE VendorAuditVendorId = VendorId
					AND VendorAuditActionId = 1 -- Created
				ORDER BY VendorAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 VendorAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM VendorAudit
					LEFT JOIN [User] ON VendorAuditUserId = UserId
				WHERE VendorAuditVendorId = VendorId
					AND VendorAuditActionId = 2 -- Update
				ORDER BY VendorAuditDate DESC
			) AS AuditModified

	WHERE VendorIsDeleted = 0;