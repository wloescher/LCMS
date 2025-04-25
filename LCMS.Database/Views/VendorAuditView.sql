CREATE VIEW VendorAuditView AS
	
	SELECT VendorAuditId
		, ActionId = VendorAuditActionId
		, [Action] = AuditAction.DataDictionaryKey
		, VendorId = VendorAuditVendorId
		, UserId = VendorAuditUserId
		, VendorName
		, UserFullName = UserFirstName + ' ' + UserLastName
		, UserEmailAddress
		, [Date] = VendorAuditDate
		, BeforeJson = VendorAuditBeforeJson
		, AfterJson = VendorAuditAfterJson
		, AffectedColumns = VendorAuditAffectedColumns

	FROM VendorAudit
		LEFT JOIN DataDictionary AS AuditAction ON DataDictionaryGroupId = 1 -- AuditAction
			AND VendorAuditActionId = AuditAction.DataDictionaryValue
		LEFT JOIN [Vendor] ON VendorAuditVendorId = VendorId
		LEFT JOIN [User] ON VendorAuditUserId = UserId;