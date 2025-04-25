using LCMS.Models;

namespace LCMS.Services.Interfaces
{
    public interface IVendorService
    {
        VendorModel? CreateVendor(VendorModel model, int userId);
        VendorModel? GetVendor(int vendorId);
        List<VendorModel> GetVendors(bool activeOnly = true, bool resetCache = false);
        bool UpdateVendor(VendorModel model, int userId);
        bool DeleteVendor(int vendorId, int userId);

        bool CheckForUniqueVendorName(int vendorId, string clientName);
        List<KeyValuePair<int, string>> GetVendorKeyValuePairs(bool activeOnly = true);

        bool CreateVendorComment(int vendorId, string comment, int userId);
        List<CommentModel> GetVendorComments(int vendorId);
        bool DeleteVendorComment(int vendorCommentId, int userId);

        bool CreateVendorDocument(int vendorId, DocumentModel model, int userId);
        List<DocumentModel> GetVendorDocuments(int vendorId);
        bool DeleteVendorDocument(int vendorDocumentId, int userId);

        bool CreateVendorNote(int vendorId, string notes, int userId);
        List<NoteModel> GetVendorNotes(int vendorId);
        bool DeleteVendorNote(int vendorNoteId, int userId);

        bool CreateVendorUser(int vendorId, int userId, int userId_Source);
        List<UserModel> GetVendorUsers(int vendorId);
        bool DeleteVendorUser(int vendorId, int userId, int userId_Source);
    }
}
