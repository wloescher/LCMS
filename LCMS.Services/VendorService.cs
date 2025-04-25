using LCMS.Models;
using LCMS.Repository.Entities;
using LCMS.Services.BaseClasses;
using LCMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LCMS.Services
{
    public class VendorService(IDbContextFactory<LCMSDatabaseContext> dbContextFactory, IMemoryCache memoryCache, IServiceProvider serviceProvider, IConfiguration configuration)
        : DbContextService(dbContextFactory, memoryCache, serviceProvider, configuration), IVendorService
    {
        #region Public Methods

        /// <summary>
        /// Add a new vendor to the database.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns>New VendorModel object.</returns>
        public VendorModel? CreateVendor(VendorModel model, int userId)
        {
            var entity = new Vendor
            {
                VendorTypeId = (int)model.Type,
                VendorName = model.Name.Trim(),
                VendorAddressLine1 = model.AddressLine1?.Trim(),
                VendorAddressLine2 = model.AddressLine2?.Trim(),
                VendorCity = model.City?.Trim(),
                VendorRegion = model.Region?.Trim(),
                VendorPostalCode = model.PostalCode?.Trim(),
                VendorCountry = model.Country?.Trim(),
                VendorPhoneNumber = model.PhoneNumber?.Trim(),
                VendorUrl = model.Url?.Trim(),
            };

            bool dbUpdated;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                dbContext.Vendors.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateVendor(entity, userId);
            }

            return GetModel(entity);
        }

        /// <summary>
        /// Get vendor.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns>VendorModel object.</returns>
        public VendorModel? GetVendor(int vendorId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var entity = dbContext.Vendors.Find(vendorId);
            return GetModel(entity);
        }

        /// <summary>
        /// Get vendors.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <param name="resetCache"></param>
        /// <returns>Collection of VendorModel objects.</returns>
        public List<VendorModel> GetVendors(bool activeOnly = true, bool resetCache = false)
        {
            // Get model from cache
            var cacheKey = string.Format("LCMS-Vendors-{0}", !activeOnly ? "IncludingInactive" : string.Empty);
            var models = _memoryCache.Get(cacheKey) as List<VendorModel>;
            if (models == null || resetCache)
            {
                List<Vendor> entities;

                // Check for active
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    entities = activeOnly
                        ? dbContext.Vendors.Where(x => !x.VendorIsDeleted && x.VendorIsActive).ToList()
                        : dbContext.Vendors.Where(x => !x.VendorIsDeleted).ToList();
                }

                // Convert to models
                models = new List<VendorModel>();
                foreach (var entity in entities.OrderBy(x => x.VendorName))
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
        /// Save a vendor.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool UpdateVendor(VendorModel model, int userId)
        {
            var dbUpdated = false;
            Vendor entityBefore = new();
            Vendor entityAfter = new();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                // Get entity
                entityBefore = dbContext.Vendors.Find(model.Id) ?? new();
                if (entityBefore.VendorId != 0)
                {
                    // Check for a name change
                    if (entityBefore.VendorName.ToLower().Trim() != model.Name.ToLower().Trim())
                    {
                        // Check for unique vendor name
                        var vendorNameIsUnique = CheckForUniqueVendorName(model.Id, model.Name);
                        if (!vendorNameIsUnique)
                        {
                            throw new ApplicationException("Vendor Name already exists - must be unique.");
                        }
                    }

                    // Update entity property values
                    entityAfter = entityBefore;
                    entityAfter.VendorIsActive = model.IsActive;
                    entityAfter.VendorName = model.Name.Trim();
                    entityAfter.VendorAddressLine1 = model.AddressLine1?.Trim();
                    entityAfter.VendorAddressLine2 = model.AddressLine2?.Trim();
                    entityAfter.VendorCity = model.City?.Trim();
                    entityAfter.VendorRegion = model.Region?.Trim();
                    entityAfter.VendorPostalCode = model.PostalCode?.Trim();
                    entityAfter.VendorCountry = model.Country?.Trim();
                    entityAfter.VendorPhoneNumber = model.PhoneNumber?.Trim();
                    entityAfter.VendorUrl = model.Url?.Trim();

                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.UpdateVendor(entityBefore, entityAfter, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Delete a vendor from the database.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteVendor(int vendorId, int userId)
        {
            var dbUpdated = false;
            Vendor entityBefore = new();
            Vendor entityAfter = new();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                // Get entity
                entityBefore = dbContext.Vendors.Find(vendorId) ?? new();
                if (entityBefore.VendorId != 0)
                {
                    // Update entity property values
                    entityAfter = entityBefore;
                    entityAfter.VendorIsDeleted = true;

                    dbUpdated = dbContext.SaveChanges() > 0;

                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteVendor(entityBefore, entityAfter, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Check if the vendor name is unique.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="vendorName"></param>
        /// <returns><c>true</c> if unique, otherwise <c>false</c>.</returns>
        public bool CheckForUniqueVendorName(int vendorId, string vendorName)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return !dbContext.Vendors.Any(x => x.VendorId != vendorId && x.VendorName.ToLower() == vendorName.ToLower().Trim());
        }

        /// <summary>
        /// Get vendor key/value pairs.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <param name="excludeInternal"></param>
        /// <returns>Collection of key/value pairs.</returns>
        public List<KeyValuePair<int, string>> GetVendorKeyValuePairs(bool activeOnly = true)
        {
            var models = GetVendors(activeOnly);

            var keyValuePairs = new List<KeyValuePair<int, string>>();
            foreach (var model in models)
            {
                keyValuePairs.Add(new KeyValuePair<int, string>(model.Id, model.Name));
            }

            return keyValuePairs;
        }

        /// <summary>
        /// Get Vendor Comment.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<CommentModel> GetVendorComments(int vendorId)
        {
            // Get entities
            List<VendorCommentView> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.VendorCommentViews.Where(x => x.VendorId == vendorId).ToList();
            }

            // Convert to models
            var models = new List<CommentModel>();
            foreach (var entity in entities.OrderBy(x => x.VendorId))
            {
                var model = GetModel(entity);
                if (model != null)
                {
                    models.Add(model);
                }
            }
            return models;
        }

        /// <summary>
        /// Add a Comment to a Vendor.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateVendorComment(int vendorId, string comment, int userId)
        {
            var dbUpdated = false;
            VendorComment? entity;

            // Check for existing Vendor user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = new VendorComment
                {
                    VendorCommentVendorId = vendorId,
                    VendorCommentBody = comment,
                };

                dbContext.VendorComments.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateVendorComment(entity, userId);
            }

            return true;
        }

        /// <summary>
        /// Delete a Comment from a Vendor.
        /// </summary>
        /// <param name="vendorCommentId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteVendorComment(int vendorCommentId, int userId)
        {
            var dbUpdated = false;
            VendorComment entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.VendorComments.Find(vendorCommentId) ?? new();
                if (entity.VendorCommentId != 0)
                {
                    entity.VendorCommentIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteVendorComment(entity, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Get Vendor Documents.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<DocumentModel> GetVendorDocuments(int vendorId)
        {
            // Get entities
            List<VendorDocument> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.VendorDocuments.Where(x => x.VendorDocumentVendorId == vendorId && !x.VendorDocumentIsDeleted).ToList();
            }

            // Convert to models
            var models = new List<DocumentModel>();
            foreach (var entity in entities.OrderBy(x => x.VendorDocumentId))
            {
                var model = GetModel(entity);
                if (model != null)
                {
                    models.Add(model);
                }
            }
            return models;
        }

        /// <summary>
        /// Add a Document to a Vendor.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="model"></param>
        /// <param name="userId_Source"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateVendorDocument(int vendorId, DocumentModel model, int userId_Source)
        {
            var dbUpdated = false;
            VendorDocument? entity;

            // Check for existing Vendor user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = new VendorDocument
                {
                    VendorDocumentVendorId = vendorId,
                    VendorDocumentTypeId = (int)model.Type,
                    VendorDocumentTitle = model.Title,
                    VendorDocumentOriginalFileName = model.OriginalFileName,
                };

                dbContext.VendorDocuments.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateVendorDocument(entity, userId_Source);
            }

            return true;
        }

        /// <summary>
        /// Delete a Document from a Vendor.
        /// </summary>
        /// <param name="vendorDocumentId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteVendorDocument(int vendorDocumentId, int userId)
        {
            var dbUpdated = false;
            VendorDocument entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.VendorDocuments.Find(vendorDocumentId) ?? new();
                if (entity.VendorDocumentId != 0)
                {
                    entity.VendorDocumentIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteVendorDocument(entity, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Get Vendor Notes.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<NoteModel> GetVendorNotes(int vendorId)
        {
            // Get entities
            List<VendorNoteView> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.VendorNoteViews.Where(x => x.VendorId == vendorId).ToList();
            }

            // Convert to models
            var models = new List<NoteModel>();
            foreach (var entity in entities.OrderBy(x => x.VendorId))
            {
                var model = GetModel(entity);
                if (model != null)
                {
                    models.Add(model);
                }
            }
            return models;
        }

        /// <summary>
        /// Add a Note to a Vendor.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="note"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateVendorNote(int vendorId, string note, int userId)
        {
            var dbUpdated = false;
            VendorNote? entity;

            // Check for existing Vendor user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = new VendorNote
                {
                    VendorNoteVendorId = vendorId,
                    VendorNoteBody = note,
                };

                dbContext.VendorNotes.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateVendorNote(entity, userId);
            }

            return true;
        }

        /// <summary>
        /// Delete a Note from a Vendor.
        /// </summary>
        /// <param name="vendorNoteId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteVendorNote(int vendorNoteId, int userId)
        {
            var dbUpdated = false;
            VendorNote entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.VendorNotes.Find(vendorNoteId) ?? new();
                if (entity.VendorNoteId != 0)
                {
                    entity.VendorNoteIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteVendorNote(entity, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Get Vendor users.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<UserModel> GetVendorUsers(int vendorId)
        {
            // Get entities
            List<User> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.VendorUsers
                    .Join(dbContext.Users,
                        vendorUser => vendorUser.VendorUserUserId,
                        user => user.UserId,
                        (vendorUsers, users) => new { VendorUsers = vendorUsers, User = users })
                    .Where(x => !x.VendorUsers.VendorUserIsDeleted && x.VendorUsers.VendorUserVendorId == vendorId)
                    .Select(x => x.User)
                    .Where(x => !x.UserIsDeleted)
                    .ToList();
            }

            // Convert to models
            var models = new List<UserModel>();
            foreach (var entity in entities.OrderBy(x => x.UserLastName).ThenBy(x => x.UserFirstName))
            {
                var model = UserService.GetModel(entity);
                if (model != null)
                {
                    models.Add(model);
                }
            }
            return models;
        }

        /// <summary>
        /// Add a user to a Vendor.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="userId"></param>
        /// <param name="userId_Source"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateVendorUser(int vendorId, int userId, int userId_Source)
        {
            var dbUpdated = false;
            VendorUser? entity;

            // Check for existing Vendor user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.VendorUsers.FirstOrDefault(x => x.VendorUserVendorId == vendorId && x.VendorUserUserId == userId);
                if (entity == null)
                {
                    entity = new VendorUser
                    {
                        VendorUserVendorId = vendorId,
                        VendorUserUserId = userId,
                    };

                    dbContext.VendorUsers.Add(entity);
                }
                else
                {
                    entity.VendorUserIsDeleted = false;
                }

                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateVendorUser(entity, userId_Source);
            }

            return true;
        }

        /// <summary>
        /// Delete a user from a Vendor.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="userId"></param>
        /// <param name="userId_Source"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteVendorUser(int vendorId, int userId, int userId_Source)
        {
            var dbUpdated = false;
            VendorUser entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.VendorUsers.FirstOrDefault(x => x.VendorUserVendorId == vendorId && x.VendorUserUserId == userId) ?? new();
                if (entity.VendorUserId != 0)
                {
                    entity.VendorUserIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteVendorUser(entity, userId_Source);
            }

            return dbUpdated;
        }

        #endregion

        #region Private Methods

        private static VendorModel? GetModel(Vendor? entity)
        {
            if (entity == null) return null;

            var model = new VendorModel
            {
                Id = entity.VendorId,
                Guid = entity.VendorGuid,
                TypeId = entity.VendorTypeId,
                IsActive = entity.VendorIsActive,
                IsDeleted = entity.VendorIsDeleted,
                Name = entity.VendorName,
                AddressLine1 = entity.VendorAddressLine1,
                AddressLine2 = entity.VendorAddressLine2,
                City = entity.VendorCity,
                Region = entity.VendorRegion,
                PostalCode = entity.VendorPostalCode,
                Country = entity.VendorCountry,
                PhoneNumber = entity.VendorPhoneNumber,
                Url = entity.VendorUrl,
            };

            return model;
        }

        internal static VendorModel? GetModel(VendorView? entity)
        {
            if (entity == null) return null;

            var model = new VendorModel
            {
                Id = entity.VendorId,
                Guid = entity.Guid,
                TypeId = entity.TypeId,
                IsActive = entity.IsActive,
                Name = entity.Name,
                AddressLine1 = entity.AddressLine1,
                AddressLine2 = entity.AddressLine2,
                City = entity.City,
                Region = entity.Region,
                PostalCode = entity.PostalCode,
                Country = entity.Country,
                PhoneNumber = entity.PhoneNumber,
                Url = entity.Url,
                //CreatedDate = entity.CreatedDate,
                //CreatedBy = entity.CreatedBy,
                //ModifiedDate = entity.ModifiedDate,
                //ModifiedBy = entity.ModifiedBy,
            };

            return model;
        }

        private static CommentModel? GetModel(VendorCommentView entity)
        {
            if (entity == null) return null;

            var model = new CommentModel
            {
                Id = entity.VendorCommentId,
                Comment = entity.Comment,
                //CreatedDate = entity.CreatedDate,
                //CreatedBy = entity.CreatedBy,
                //ModifiedDate = entity.ModifiedDate,
                //ModifiedBy = entity.ModifiedBy,
            };

            return model;
        }

        private static DocumentModel? GetModel(VendorDocument entity)
        {
            if (entity == null) return null;

            var model = new DocumentModel
            {
                Id = entity.VendorDocumentId,
                TypeId = entity.VendorDocumentTypeId,
                Title = entity.VendorDocumentTitle,
                Summary = entity.VendorDocumentSummary,
                OriginalFileName = entity.VendorDocumentOriginalFileName,
                //CreatedDate = entity.CreatedDate,
                //CreatedBy = entity.CreatedBy,
                //ModifiedDate = entity.ModifiedDate,
                //ModifiedBy = entity.ModifiedBy,
            };

            return model;
        }

        private static DocumentModel? GetModel(VendorDocumentView entity)
        {
            if (entity == null) return null;

            var model = new DocumentModel
            {
                Id = entity.VendorDocumentId,
                TypeId = entity.VendorDocumentTypeId,
                Title = entity.DocumentTitle,
                Summary = entity.DocumentSummary,
                OriginalFileName = entity.OriginalFileName,
                //CreatedDate = entity.CreatedDate,
                //CreatedBy = entity.CreatedBy,
                //ModifiedDate = entity.ModifiedDate,
                //ModifiedBy = entity.ModifiedBy,
            };

            return model;
        }

        private static NoteModel? GetModel(VendorNoteView entity)
        {
            if (entity == null) return null;

            var model = new NoteModel
            {
                Id = entity.VendorNoteId,
                Note = entity.Note,
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
