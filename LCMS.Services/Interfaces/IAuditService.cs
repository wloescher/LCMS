using LCMS.Models;
using LCMS.Repository.Entities;

namespace LCMS.Services.Interfaces
{
    public interface IAuditService
    {
        List<AuditModel> GetDataDictionaryGroupAudits(int caseId);
        bool CreateDataDictionaryGroup(DataDictionaryGroup entity, int userId);
        bool UpdateDataDictionaryGroup(DataDictionaryGroup entityBefore, DataDictionaryGroup entityAfter, int userId);
        bool DeleteDataDictionaryGroup(DataDictionaryGroup entityBefore, DataDictionaryGroup entityAfter, int userId);

        List<AuditModel> GetDataDictionaryAudits(int caseId);
        bool CreateDataDictionary(DataDictionary entity, int userId);
        bool UpdateDataDictionary(DataDictionary entityBefore, DataDictionary entityAfter, int userId);
        bool DeleteDataDictionary(DataDictionary entityBefore, DataDictionary entityAfter, int userId);

        List<AuditModel> GetCaseAudits(int caseId);
        bool CreateCase(Case entity, int userId);
        bool UpdateCase(Case entityBefore, Case entityAfter, int userId);
        bool DeleteCase(Case entityBefore, Case entityAfter, int userId);

        List<AuditModel> GetCaseCommentAudits(int caseCommentId);
        bool CreateCaseComment(CaseComment entity, int CommentId);
        bool DeleteCaseComment(CaseComment entity, int CommentId);

        List<AuditModel> GetCaseDocumentAudits(int caseDocumentId);
        bool CreateCaseDocument(CaseDocument entity, int DocumentId);
        bool DeleteCaseDocument(CaseDocument entity, int DocumentId);

        List<AuditModel> GetCaseNoteAudits(int caseNoteId);
        bool CreateCaseNote(CaseNote entity, int NoteId);
        bool DeleteCaseNote(CaseNote entity, int NoteId);

        List<AuditModel> GetCaseUserAudits(int caseUserId);
        bool CreateCaseUser(CaseUser entity, int userId);
        bool DeleteCaseUser(CaseUser entity, int userId);

        List<AuditModel> GetClientAudits(int clientId);
        bool CreateClient(Client entity, int userId);
        bool UpdateClient(Client entityBefore, Client entityAfter, int userId);
        bool DeleteClient(Client entityBefore, Client entityAfter, int userId);

        List<AuditModel> GetContractAudits(int contractId);
        bool CreateContract(Contract entity, int userId);
        bool UpdateContract(Contract entityBefore, Contract entityAfter, int userId);
        bool DeleteContract(Contract entityBefore, Contract entityAfter, int userId);

        List<AuditModel> GetContractCommentAudits(int contractCommentId);
        bool CreateContractComment(ContractComment entity, int CommentId);
        bool DeleteContractComment(ContractComment entity, int CommentId);

        List<AuditModel> GetContractDocumentAudits(int contractDocumentId);
        bool CreateContractDocument(ContractDocument entity, int DocumentId);
        bool DeleteContractDocument(ContractDocument entity, int DocumentId);

        List<AuditModel> GetContractNoteAudits(int contractNoteId);
        bool CreateContractNote(ContractNote entity, int NoteId);
        bool DeleteContractNote(ContractNote entity, int NoteId);

        List<AuditModel> GetContractUserAudits(int contractUserId);
        bool CreateContractUser(ContractUser entity, int userId);
        bool DeleteContractUser(ContractUser entity, int userId);

        List<AuditModel> GetUserAudits(int userId);
        bool CreateUser(User entity, int userId);
        bool UpdateUser(User entityBefore, User entityAfter, int userId);
        bool DeleteUser(User entityBefore, User entityAfter, int userId);

        List<AuditModel> GetUserAccountAudits(int caseId);
        bool CreateUserAccount(UserAccount entity, int userId);
        bool UpdateUserAccount(UserAccount entityBefore, UserAccount entityAfter, int userId);
        bool DeleteUserAccount(UserAccount entityBefore, UserAccount entityAfter, int userId);

        List<AuditModel> GetVendorAudits(int vendorId);
        bool CreateVendor(Vendor entity, int userId);
        bool UpdateVendor(Vendor entityBefore, Vendor entityAfter, int userId);
        bool DeleteVendor(Vendor entityBefore, Vendor entityAfter, int userId);

        List<AuditModel> GetVendorCommentAudits(int vendorCommentId);
        bool CreateVendorComment(VendorComment entity, int CommentId);
        bool DeleteVendorComment(VendorComment entity, int CommentId);

        List<AuditModel> GetVendorDocumentAudits(int vendorDocumentId);
        bool CreateVendorDocument(VendorDocument entity, int DocumentId);
        bool DeleteVendorDocument(VendorDocument entity, int DocumentId);

        List<AuditModel> GetVendorNoteAudits(int vendorNoteId);
        bool CreateVendorNote(VendorNote entity, int NoteId);
        bool DeleteVendorNote(VendorNote entity, int NoteId);

        List<AuditModel> GetVendorUserAudits(int vendorUserId);
        bool CreateVendorUser(VendorUser entity, int userId);
        bool DeleteVendorUser(VendorUser entity, int userId);
    }
}
