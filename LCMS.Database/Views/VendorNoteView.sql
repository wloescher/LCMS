CREATE VIEW VendorNoteView AS
	
	SELECT VendorNoteId
		, VendorNoteGuid
		, VendorId = VendorNoteVendorId
		, VendorName
		, Note = VendorNoteBody
		, CreatedDate = IsNull(AuditCreated.VendorNoteAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.VendorNoteAuditDate, IsNull(AuditCreated.VendorNoteAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM VendorNote
		LEFT JOIN [Vendor] ON VendorNoteVendorId = VendorId
			AND VendorIsActive = 1
			AND VendorIsDeleted = 0
		OUTER APPLY 
			(
				SELECT TOP 1 VendorNoteAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM VendorNoteAudit
					LEFT JOIN [User] ON VendorNoteAuditUserId = UserId
				WHERE VendorNoteAuditVendorNoteId = VendorNoteId
					AND VendorNoteAuditActionId = 1 -- Created
				ORDER BY VendorNoteAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 VendorNoteAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM VendorNoteAudit
					LEFT JOIN [User] ON VendorNoteAuditUserId = UserId
				WHERE VendorNoteAuditVendorNoteId = VendorNoteId
					AND VendorNoteAuditActionId = 2 -- Update
				ORDER BY VendorNoteAuditDate DESC
			) AS AuditModified
			
	WHERE VendorNoteIsDeleted = 0;