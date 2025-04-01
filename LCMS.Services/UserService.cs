using LCMS.Models;
using LCMS.Repository.Entities;
using LCMS.Services.BaseClasses;
using LCMS.Services.Interfaces;
using LCMS.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Text;

namespace LCMS.Services
{
    public class UserService(IDbContextFactory<LCMSDatabaseContext> dbContextFactory, IMemoryCache memoryCache, IServiceProvider serviceProvider, IConfiguration configuration)
        : DbContextService(dbContextFactory, memoryCache, serviceProvider, configuration), IUserService
    {
        #region Public Methods

        /// <summary>
        /// Get a user by username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>null if not found or password is incorrect.</returns>
        public UserModel? GetUser(string username, string password)
        {
            UserModel? model = null;

            UserAccount? userAccountEntity = null;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                userAccountEntity = dbContext.UserAccounts.FirstOrDefault(x => x.UserAccountUserName == username && !x.UserAccountIsDeleted && x.UserAccountIsActive);
            }

            // Check password
            if (userAccountEntity != null && SecurityUtility.PasswordHashVerify(password, userAccountEntity.UserAccountPasswordHash))
            {
                User? userEntity = null;
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    userEntity = dbContext.Users.FirstOrDefault(x => x.UserUserAccountId == userAccountEntity.UserAccountId && !x.UserIsDeleted && x.UserIsActive);
                }

                model = GetModel(userEntity);
                if (model != null)
                {
                    model.UserName = userAccountEntity.UserAccountUserName;
                }
            }

            return model;
        }

        /// <summary>
        /// Get the currently logged in user's UserId.
        /// </summary>
        /// <returns>UserId of logged in User</returns>
        public int GetCurrentUserId(HttpContext httpContext)
        {
            var nameIdentifierClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return nameIdentifierClaim == null ? 0 : Convert.ToInt32(nameIdentifierClaim.Value);
        }

        /// <summary>
        /// Create a user.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <param name="errorMessage"></param>
        /// <returns>New UserModel object.</returns>
        public UserModel? CreateUser(UserModel model, int userId, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Check for existing User record
            User? userEntity = null;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                userEntity = dbContext.Users.FirstOrDefault(x => x.UserEmailAddress == model.EmailAddress.Trim());
            }

            if (userEntity != null)
            {
                errorMessage = "User already exists - please try a different Email Address.";
                return null;
            }

            // Set password
            var password = SecurityUtility.GeneratePassword();

            var entity = new User
            {
                UserTypeId = (int)model.Type,
                UserIsActive = true,
                UserFirstName = model.FirstName.Trim(),
                UserMiddleName = model.MiddleName?.Trim(),
                UserLastName = model.LastName.Trim(),
                UserEmailAddress = model.EmailAddress.Trim(),
                UserAddressLine1 = model.AddressLine1?.Trim(),
                UserAddressLine2 = model.AddressLine2?.Trim(),
                UserCity = model.City?.Trim(),
                UserRegion = model.Region?.Trim(),
                UserPostalCode = model.PostalCode?.Trim(),
                UserCountry = model.Country?.Trim(),
                UserPhoneNumber = model.PhoneNumber?.Trim(),
            };

            bool dbUpdated;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                dbContext.Users.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Create audit record
                    var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                    auditService.CreateUser(entity, userId);
                }

                // Send access info email
                var emailBody = GetChangePasswordEmailBody(model.EmailAddress, password);
                var returnMessage = string.Empty;
                var emailUtility = new EmailUtility(_configuration);
                emailUtility.SendMail(model.EmailAddress, "Access Information", emailBody, out returnMessage, true);
            }

            // Update cached data
            GetUsers(true, true);

            return GetModel(entity);
        }

        /// <summary>
        /// Get user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>UserModel object.</returns>
        public UserModel? GetUser(int userId)
        {
            User? entity;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.Users.Find(userId);
            }

            return GetModel(entity);
        }

        /// <summary>
        /// Get users.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <param name="resetCache"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<UserModel> GetUsers(bool activeOnly = true, bool resetCache = false)
        {
            // Get model from cache
            var cacheKey = string.Format("LCMS-Users-{0}", !activeOnly ? "IncludingInactive" : string.Empty);
            var models = _memoryCache.Get(cacheKey) as List<UserModel>;
            if (models == null || resetCache)
            {
                var entities = new List<UserView>();

                // Check for active
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    entities = activeOnly
                        ? dbContext.UserViews.Where(x => x.IsActive).ToList()
                        : dbContext.UserViews.ToList();
                }

                // Convert to models
                models = new List<UserModel>();
                foreach (var entity in entities.OrderBy(x => x.LastName).ThenBy(x => x.FirstName))
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

            return models
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToList();
        }

        /// <summary>
        /// Save a user.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool UpdateUser(UserModel model, int userId, out string errorMessage)
        {
            errorMessage = string.Empty;

            var dbUpdated = false;
            User entityBefore = new();
            User entityAfter = new();

            // Check for duplicate real name
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entityBefore = dbContext.Users.FirstOrDefault(x => x.UserEmailAddress == model.EmailAddress.Trim() && x.UserId != userId) ?? new();
                if (entityBefore.UserId != 0)
                {
                    errorMessage = "User already exists - please try a different Email Address.";
                    return false;
                }

                entityBefore = dbContext.Users.Find(model.Id) ?? new();
                if (entityBefore.UserId != 0)
                {
                    // Check for an email address change
                    if (entityBefore.UserEmailAddress != model.EmailAddress.ToLower().Trim())
                    {
                        // Check for unique email address
                        var emailAddressIsUnique = CheckForUniqueUserEmailAddress(model.Id, model.EmailAddress);
                        if (!emailAddressIsUnique)
                        {
                            throw new ApplicationException("Email Address already exists - must be unique.");
                        }
                    }

                    // Update entity property values
                    entityAfter = entityBefore;
                    entityAfter.UserTypeId = (int)model.Type;
                    entityAfter.UserIsActive = model.IsActive;
                    entityAfter.UserFirstName = model.FirstName?.Trim();
                    entityAfter.UserMiddleName = model.MiddleName?.Trim();
                    entityAfter.UserLastName = model.LastName?.Trim();
                    entityAfter.UserEmailAddress = model.EmailAddress.Trim();
                    entityAfter.UserAddressLine1 = model.AddressLine1?.Trim();
                    entityAfter.UserAddressLine2 = model.AddressLine2?.Trim();
                    entityAfter.UserCity = model.City?.Trim();
                    entityAfter.UserRegion = model.Region?.Trim();
                    entityAfter.UserPostalCode = model.PostalCode?.Trim();
                    entityAfter.UserCountry = model.Country?.Trim();
                    entityAfter.UserPhoneNumber = model.PhoneNumber?.Trim();

                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Create audit record
                    var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                    auditService.UpdateUser(entityBefore, entityAfter, userId);
                }

                // Update cached data
                GetUsers(true, true);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Delete a user from the database.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userId_Source"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteUser(int userId, int userId_Source)
        {
            var dbUpdated = false;
            User entityBefore = new();
            User entityAfter = new();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entityBefore = dbContext.Users.Find(userId) ?? new();
                if (entityBefore.UserId != 0)
                {
                    // Update entity property values
                    entityAfter = entityBefore;
                    entityAfter.UserIsDeleted = true;

                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteUser(entityBefore, entityAfter, userId_Source);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Check if the user email address is unique.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="emailAddress"></param>
        /// <returns><c>true</c> if unique, otherwise <c>false</c>.</returns>
        public bool CheckForUniqueUserEmailAddress(int userId, string emailAddress)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return !dbContext.Users.Any(x => x.UserId != userId && x.UserEmailAddress.ToLower() == emailAddress.ToLower().Trim());
        }

        /// <summary>
        /// Get user key/value pairs.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <param name="excludeInternal"></param>
        /// <returns>Collection of key/value pairs.</returns>
        public List<KeyValuePair<int, string>> GetUserKeyValuePairs(bool activeOnly = true, bool excludeInternal = true)
        {
            var models = GetUsers(activeOnly, excludeInternal);

            var keyValuePairs = new List<KeyValuePair<int, string>>();
            foreach (var model in models)
            {
                keyValuePairs.Add(new KeyValuePair<int, string>(model.Id, model.FullName));
            }

            return keyValuePairs;
        }

        /// <summary>
        /// Get user cases.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Collection of CaseModel objects.</returns>
        public List<CaseModel> GetUserCases(int userId)
        {
            // Get entities
            List<CaseView> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.CaseUserViews
                    .Join(dbContext.CaseViews,
                        caseUserView => caseUserView.CaseId,
                        caseView => caseView.CaseId,
                        (caseUserView, caseView) => new { CaseUserView = caseUserView, CaseView = caseView })
                    .Where(x => x.CaseUserView.UserId == userId)
                    .Select(x => x.CaseView)
                    .ToList();
            }

            // Convert to models
            var models = new List<CaseModel>();
            foreach (var entity in entities.OrderBy(x => x.Title))
            {
                var model = CaseService.GetModel(entity);
                if (model != null)
                {
                    models.Add(model);
                }
            }
            return models;
        }

        #endregion

        #region Private Methods

        private static UserModel? GetModel(User? entity)
        {
            if (entity == null) return null;

            var model = new UserModel
            {
                Id = entity.UserId,
                Guid = entity.UserGuid,
                TypeId = entity.UserTypeId,
                IsActive = entity.UserIsActive,
                IsDeleted = entity.UserIsDeleted,
                FirstName = entity.UserFirstName,
                MiddleName = entity.UserMiddleName,
                LastName = entity.UserLastName,
                EmailAddress = entity.UserEmailAddress,
                AddressLine1 = entity.UserAddressLine1,
                AddressLine2 = entity.UserAddressLine2,
                City = entity.UserCity,
                Region = entity.UserRegion,
                PostalCode = entity.UserPostalCode,
                Country = entity.UserCountry,
                PhoneNumber = entity.UserPhoneNumber,
            };

            return model;
        }

        internal static UserModel? GetModel(UserView? entity)
        {
            if (entity == null) return null;

            var model = new UserModel
            {
                Id = entity.UserId,
                Guid = entity.Guid,
                TypeId = entity.TypeId,
                IsActive = entity.IsActive,
                UserName = entity.UserName,
                FirstName = entity.FirstName,
                MiddleName = entity.MiddleName,
                LastName = entity.LastName,
                EmailAddress = entity.EmailAddress,
                AddressLine1 = entity.AddressLine1,
                AddressLine2 = entity.AddressLine2,
                City = entity.City,
                Region = entity.Region,
                PostalCode = entity.PostalCode,
                Country = entity.Country,
                PhoneNumber = entity.PhoneNumber,
                //CreatedDate = entity.CreatedDate,
                //CreatedBy = entity.CreatedBy,
                //ModifiedDate = entity.ModifiedDate,
                //ModifiedBy = entity.ModifiedBy,
            };

            return model;
        }

        #endregion

        #region Private Methods

        private static string GetChangePasswordEmailBody(string username, string password)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<p>Here is your access information for lcms.com.</p>");
            sb.AppendFormat("<p>You can sign into the website by visiting <a href=\"https://www.lcms.com\">lcms.com</a> and entering your information.</p>");
            sb.AppendFormat("<div><strong>Username:</strong> {0}</div>", username);
            sb.AppendFormat("<div><strong>Password:</strong> {0}</div>", password);
            return sb.ToString();
        }

        #endregion
    }
}
