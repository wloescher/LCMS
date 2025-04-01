using LCMS.Models;
using LCMS.Repository.Entities;
using LCMS.Services.BaseClasses;
using LCMS.Services.Interfaces;
using LCMS.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LCMS.Services
{
    public class UserAccountService(IDbContextFactory<LCMSDatabaseContext> dbContextFactory, IMemoryCache memoryCache, IServiceProvider serviceProvider, IConfiguration configuration)
        : DbContextService(dbContextFactory, memoryCache, serviceProvider, configuration), IUserAccountService
    {
        #region Public Methods

        /// <summary>
        /// Add a new user account to the database.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <returns>New UserAccountModel object.</returns>
        public UserAccountModel? CreateUserAccount(string userName, int userId)
        {
            var newPassword = SecurityUtility.GeneratePassword();
            var entity = new UserAccount
            {
                UserAccountIsActive = true,
                UserAccountUserName = userName.Trim(),
                UserAccountPassword = newPassword,
                UserAccountPasswordHash = SecurityUtility.PasswordHash(newPassword),
            };

            bool dbUpdated;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                dbContext.UserAccounts.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateUserAccount(entity, userId);
            }

            return GetModel(entity);
        }

        /// <summary>
        /// Get user account.
        /// </summary>
        /// <param name="userAccountId"></param>
        /// <returns>UserAccountModel object.</returns>
        public UserAccountModel? GetUserAccount(int userAccountId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var entity = dbContext.UserAccounts.Find(userAccountId);
            return GetModel(entity);
        }

        /// <summary>
        /// Get user accounts.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <param name="resetCache"></param>
        /// <returns>Collection of UserAccountModel objects.</returns>
        public List<UserAccountModel> GetUserAccounts(bool activeOnly = true, bool resetCache = false)
        {
            // Get model from cache
            var cacheKey = string.Format("LCMS-UserAccounts-{0}", !activeOnly ? "IncludingInactive" : string.Empty);
            var models = _memoryCache.Get(cacheKey) as List<UserAccountModel>;
            if (models == null || resetCache)
            {
                List<UserAccountView> entities;

                // Check for active
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    entities = activeOnly
                        ? dbContext.UserAccountViews.Where(x => x.IsActive).ToList()
                        : dbContext.UserAccountViews.ToList();
                }

                // Convert to models
                models = new List<UserAccountModel>();
                foreach (var entity in entities.OrderBy(x => x.UserName))
                {
                    var model = GetModel(entity);
                    if (model != null)
                    {
                        models.Add(model);
                    }
                }

                // Update cache
                _memoryCache.Set(cacheKey, models, _cacheOptions);
            }

            return models;
        }

        /// <summary>
        /// Save a user account.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool UpdateUserAccount(UserAccountModel model, int userId)
        {
            var dbUpdated = false;
            UserAccount entityBefore = new();
            UserAccount entityAfter = new();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                // Get entity
                entityBefore = dbContext.UserAccounts.Find(model.Id) ?? new();
                if (entityBefore.UserAccountId != 0)
                {
                    // Check for a name change
                    if (entityBefore.UserAccountUserName.ToLower().Trim() != model.UserName.ToLower().Trim())
                    {
                        // Check for unique user name
                        var userNameIsUnique = CheckForUniqueUserName(model.Id, model.UserName);
                        if (!userNameIsUnique)
                        {
                            throw new ApplicationException("UserName already exists - must be unique.");
                        }
                    }

                    // Update entity property values
                    entityAfter = entityBefore;
                    entityAfter.UserAccountIsActive = model.IsActive;
                    entityAfter.UserAccountUserName = model.UserName.Trim();

                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.UpdateUserAccount(entityBefore, entityAfter, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Delete a user account from the database.
        /// </summary>
        /// <param name="userAccountId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteUserAccount(int userAccountId, int userId)
        {
            var dbUpdated = false;
            UserAccount entityBefore = new();
            UserAccount entityAfter = new();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                // Get entity
                entityBefore = dbContext.UserAccounts.Find(userAccountId) ?? new();
                if (entityBefore.UserAccountId != 0)
                {
                    // Update entity property values
                    entityAfter = entityBefore;
                    entityAfter.UserAccountIsDeleted = true;

                    dbUpdated = dbContext.SaveChanges() > 0;

                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteUserAccount(entityBefore, entityAfter, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Check if the user name is unique.
        /// </summary>
        /// <param name="userAccountId"></param>
        /// <param name="userName"></param>
        /// <returns><c>true</c> if unique, otherwise <c>false</c>.</returns>
        public bool CheckForUniqueUserName(int userAccountId, string userName)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return !dbContext.UserAccounts.Any(x => x.UserAccountId != userAccountId && x.UserAccountUserName.ToLower() == userName.ToLower().Trim());
        }

        /// <summary>
        /// Get user account key/value pairs.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <returns>Collection of key/value pairs.</returns>
        public List<KeyValuePair<int, string>> GetUserAccountKeyValuePairs(bool activeOnly = true)
        {
            var models = GetUserAccounts(activeOnly);

            var keyValuePairs = new List<KeyValuePair<int, string>>();
            foreach (var model in models)
            {
                keyValuePairs.Add(new KeyValuePair<int, string>(model.Id, model.UserName));
            }

            return keyValuePairs;
        }

        #endregion

        #region Private Methods

        private static UserAccountModel? GetModel(UserAccount? entity)
        {
            if (entity == null) return null;

            var model = new UserAccountModel
            {
                Id = entity.UserAccountId,
                Guid = entity.UserAccountGuid,
                IsActive = entity.UserAccountIsActive,
                IsDeleted = entity.UserAccountIsDeleted,
                UserName = entity.UserAccountUserName,
                Password = entity.UserAccountPassword,
                PasswordHash = entity.UserAccountPasswordHash,
                PasswordAttemptCount = entity.UserAccountPasswordAttemptCount,
            };

            return model;
        }

        internal static UserAccountModel? GetModel(UserAccountView? entity)
        {
            if (entity == null) return null;

            var model = new UserAccountModel
            {
                Id = entity.UserAccountId,
                Guid = entity.Guid,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                UserName = entity.UserName,
                Password = entity.Password,
                PasswordHash = entity.PasswordHash,
                PasswordAttemptCount = entity.PasswordAttemptCount,
                //CreatedDate = entity.CreatedDate,
                //CreatedBy = entity.CreatedBy,
                //ModifiedDate = entity.ModifiedDate,
                //ModifiedBy = entity.ModifiedBy,
            };

            return model;
        }

        #endregion
    }
}
