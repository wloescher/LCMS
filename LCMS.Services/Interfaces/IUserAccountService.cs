using LCMS.Models;

namespace LCMS.Services.Interfaces
{
    public interface IUserAccountService
    {
        UserAccountModel? CreateUserAccount(string userName, int userId);
        UserAccountModel? GetUserAccount(int userAccountId);
        List<UserAccountModel> GetUserAccounts(bool activeOnly = true, bool resetCache = false);
        bool UpdateUserAccount(UserAccountModel model, int userId);
        bool DeleteUserAccount(int userAccountId, int userId);

        bool CheckForUniqueUserName(int userAccountId, string useName);
        List<KeyValuePair<int, string>> GetUserAccountKeyValuePairs(bool activeOnly = true);
    }
}
