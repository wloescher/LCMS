using LCMS.Models;

namespace LCMS.Services.Interfaces
{
    public interface IContractService
    {
        ContractModel? CreateContract(ContractModel model, int userId);
        ContractModel? GetContract(int contractId);
        List<ContractModel> GetContracts(bool activeOnly = true, bool excludeClosed = true, bool resetCache = false);
        bool UpdateContract(ContractModel model, int userId);
        bool DeleteContract(int contractId, int userId);

        bool CheckForUniqueTitle(int contractId, string clientName);
        List<KeyValuePair<int, string>> GetContractKeyValuePairs(bool activeOnly = true, bool excludeClosed = true);

        bool CreateContractComment(int contractId, string comment, int userId);
        List<CommentModel> GetContractComments(int contractId);
        bool DeleteContractComment(int contractCommentId, int userId);

        bool CreateContractDocument(int contractId, DocumentModel model, int userId);
        List<DocumentModel> GetContractDocuments(int contractId);
        bool DeleteContractDocument(int contractDocumentId, int userId);

        bool CreateContractNote(int contractId, string notes, int userId);
        List<NoteModel> GetContractNotes(int contractId);
        bool DeleteContractNote(int contractNoteId, int userId);

        bool CreateContractUser(int contractId, int userId, int userId_Source);
        List<UserModel> GetContractUsers(int contractId);
        bool DeleteContractUser(int contractId, int userId, int userId_Source);
    }
}
