CREATE VIEW VendorCommentView AS
	
	SELECT VendorCommentId
		, VendorCommentGuid
		, VendorId = VendorCommentVendorId
		, VendorName
		, Comment = VendorCommentBody
		, CreatedDate = IsNull(AuditCreated.VendorCommentAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.VendorCommentAuditDate, IsNull(AuditCreated.VendorCommentAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))
		, IsNull(AuditCreated.VendorCommentAuditDate, '1/1/1900') AS CreatedDate2

	FROM VendorComment
		LEFT JOIN [Vendor] ON VendorCommentVendorId = VendorId
			AND VendorIsActive = 1
			AND VendorIsDeleted = 0
		OUTER APPLY 
			(
				SELECT TOP 1 VendorCommentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM VendorCommentAudit
					LEFT JOIN [User] ON VendorCommentAuditUserId = UserId
				WHERE VendorCommentAuditVendorCommentId = VendorCommentId
					AND VendorCommentAuditActionId = 1 -- Created
				ORDER BY VendorCommentAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 VendorCommentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM VendorCommentAudit
					LEFT JOIN [User] ON VendorCommentAuditUserId = UserId
				WHERE VendorCommentAuditVendorCommentId = VendorCommentId
					AND VendorCommentAuditActionId = 2 -- Update
				ORDER BY VendorCommentAuditDate DESC
			) AS AuditModified
			
	WHERE VendorCommentIsDeleted = 0;