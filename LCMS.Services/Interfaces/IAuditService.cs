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

        List<AuditModel> GetUserAudits(int userId);
        bool CreateUser(User entity, int userId);
        bool UpdateUser(User entityBefore, User entityAfter, int userId);
        bool DeleteUser(User entityBefore, User entityAfter, int userId);

        List<AuditModel> GetUserAccountAudits(int caseId);
        bool CreateUserAccount(UserAccount entity, int userId);
        bool UpdateUserAccount(UserAccount entityBefore, UserAccount entityAfter, int userId);
        bool DeleteUserAccount(UserAccount entityBefore, UserAccount entityAfter, int userId);
    }
}
