CREATE VIEW VendorUserView AS
	
	SELECT VendorUserId
		, VendorId = VendorUserVendorId
		, UserId = VendorUserUserId
		, VendorName
		, UserFullName = UserFirstName + ' ' + UserLastName
		, UserEmailAddress
		, CreatedDate = IsNull(AuditCreated.VendorUserAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.VendorUserAuditDate, IsNull(AuditCreated.VendorUserAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM VendorUser
		LEFT JOIN [Vendor] ON VendorUserVendorId = VendorId
			AND VendorIsActive = 1
			AND VendorIsDeleted = 0
		LEFT JOIN [User] ON VendorUserUserId = UserId
			AND UserIsActive = 1
			AND UserIsDeleted = 0
		OUTER APPLY 
			(
				SELECT TOP 1 VendorUserAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM VendorUserAudit
					LEFT JOIN [User] ON VendorUserAuditUserId = UserId
				WHERE VendorUserAuditVendorUserId = VendorUserId
					AND VendorUserAuditActionId = 1 -- Created
				ORDER BY VendorUserAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 VendorUserAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM VendorUserAudit
					LEFT JOIN [User] ON VendorUserAuditUserId = UserId
				WHERE VendorUserAuditVendorUserId = VendorUserId
					AND VendorUserAuditActionId = 2 -- Update
				ORDER BY VendorUserAuditDate DESC
			) AS AuditModified
			
	WHERE VendorUserIsDeleted = 0;