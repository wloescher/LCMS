CREATE VIEW ContractNoteView AS
	
	SELECT ContractNoteId
		, ContractNoteGuid
		, ContractId = ContractNoteContractId
		, ContractTitle
		, Note = ContractNoteBody
		, CreatedDate = IsNull(AuditCreated.ContractNoteAuditDate, '1/1/1900')
		, CreatedBy = IsNull(AuditCreated.UserFullName, 'System')
		, ModifiedDate = IsNull(AuditModified.ContractNoteAuditDate, IsNull(AuditCreated.ContractNoteAuditDate, '1/1/1900'))
		, ModifiedBy = IsNull(AuditModified.UserFullName, IsNull(AuditCreated.UserFullName, 'System'))

	FROM ContractNote
		LEFT JOIN [Contract] ON ContractNoteContractId = ContractId
			AND ContractIsActive = 1
			AND ContractIsDeleted = 0
		OUTER APPLY 
			(
				SELECT TOP 1 ContractNoteAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ContractNoteAudit
					LEFT JOIN [User] ON ContractNoteAuditUserId = UserId
				WHERE ContractNoteAuditContractNoteId = ContractNoteId
					AND ContractNoteAuditActionId = 1 -- Created
				ORDER BY ContractNoteAuditDate
			) AS AuditCreated
		OUTER APPLY 
			(
				SELECT TOP 1 ContractNoteAuditDate
					, UserFullName = UserFirstName + ' ' + UserLastName
				FROM ContractNoteAudit
					LEFT JOIN [User] ON ContractNoteAuditUserId = UserId
				WHERE ContractNoteAuditContractNoteId = ContractNoteId
					AND ContractNoteAuditActionId = 2 -- Update
				ORDER BY ContractNoteAuditDate DESC
			) AS AuditModified
			
	WHERE ContractNoteIsDeleted = 0;