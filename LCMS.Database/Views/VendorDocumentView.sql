CREATE VIEW VendorDocumentView AS
	
	SELECT VendorDocumentId
		, VendorDocumentGuid
		, VendorId = VendorDocumentVendorId
		, VendorName
		, VendorDocumentTypeId
		, DocumentType = DocumentType.DataDictionaryKey
		, DocumentTitle = VendorDocumentTitle
		, DocumentSummary = VendorDocumentSummary
		, OriginalFileName = VendorDocumentOriginalFileName
		, CreatedDate = IsNull(AuditCreated.VendorDocumentAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.VendorDocumentAuditDate, IsNull(AuditCreated.VendorDocumentAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM VendorDocument
		LEFT JOIN [Vendor] ON VendorDocumentVendorId = VendorId
			AND VendorIsActive = 1
			AND VendorIsDeleted = 0
		LEFT JOIN DataDictionary AS DocumentType ON DocumentType.DataDictionaryGroupId = 6 -- DocumentType
			AND VendorDocumentTypeId = DocumentType.DataDictionaryValue
		OUTER APPLY 
			(
				SELECT TOP 1 VendorDocumentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM VendorDocumentAudit
					LEFT JOIN [User] ON VendorDocumentAuditUserId = UserId
				WHERE VendorDocumentAuditVendorDocumentId = VendorDocumentId
					AND VendorDocumentAuditActionId = 1 -- Created
				ORDER BY VendorDocumentAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 VendorDocumentAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM VendorDocumentAudit
					LEFT JOIN [User] ON VendorDocumentAuditUserId = UserId
				WHERE VendorDocumentAuditVendorDocumentId = VendorDocumentId
					AND VendorDocumentAuditActionId = 2 -- Update
				ORDER BY VendorDocumentAuditDate DESC
			) AS AuditModified
			
	WHERE VendorDocumentIsDeleted = 0;