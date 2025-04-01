using LCMS.Models;

namespace LCMS.Services.Interfaces
{
    public interface ICaseService
    {
        CaseModel? CreateCase(CaseModel model, int userId);
        CaseModel? GetCase(int caseId);
        List<CaseModel> GetCases(bool activeOnly = true, bool excludeClosed = true, bool resetCache = false);
        bool UpdateCase(CaseModel model, int userId);
        bool DeleteCase(int caseId, int userId);

        bool CheckForUniqueTitle(int caseId, string clientName);
        List<KeyValuePair<int, string>> GetCaseKeyValuePairs(bool activeOnly = true, bool excludeClosed = true);

        bool CreateCaseComment(int caseId, string comment, int userId);
        List<CommentModel> GetCaseComments(int caseId);
        bool DeleteCaseComment(int caseCommentId, int userId);

        bool CreateCaseDocument(int caseId, DocumentModel model, int userId);
        List<DocumentModel> GetCaseDocuments(int caseId);
        bool DeleteCaseDocument(int caseDocumentId, int userId);

        bool CreateCaseNote(int caseId, string notes, int userId);
        List<NoteModel> GetCaseNotes(int caseId);
        bool DeleteCaseNote(int caseNoteId, int userId);

        bool CreateCaseUser(int cCaseId, int userId, int userId_Source);
        List<UserModel> GetCaseUsers(int caseId);
        bool DeleteCaseUser(int caseId, int userId, int userId_Source);

    }
}
