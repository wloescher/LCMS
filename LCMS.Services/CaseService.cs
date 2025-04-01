using LCMS.Models;
using LCMS.Repository.Entities;
using LCMS.Services.BaseClasses;
using LCMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static LCMS.Models.Enums;

namespace LCMS.Services
{
    public class CaseService(IDbContextFactory<LCMSDatabaseContext> dbContextFactory, IMemoryCache memoryCache, IServiceProvider serviceProvider, IConfiguration configuration)
        : DbContextService(dbContextFactory, memoryCache, serviceProvider, configuration), ICaseService
    {
        #region Public Methods

        /// <summary>
        /// Add a new Case to the database.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns>New CaseModel object.</returns>
        public CaseModel? CreateCase(CaseModel model, int userId)
        {
            var entity = new Case
            {
                CaseClientId = model.ClientId,
                CaseTypeId = (int)model.Type,
                CaseStatusId = (int)model.Status,
                CaseTitle = model.Title.Trim(),
                CaseSummary = model.Summary?.Trim(),
            };

            bool dbUpdated;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                dbContext.Cases.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateCase(entity, userId);
            }

            return GetModel(entity);
        }

        /// <summary>
        /// Get Case.
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns>CaseModel object.</returns>
        public CaseModel? GetCase(int caseId)
        {
            Case? entity = null;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.Cases.Find(caseId);
            }
            return GetModel(entity);
        }

        /// <summary>
        /// Get Cases.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <param name="excludeClosed"></param>
        /// <param name="resetCache"></param>
        /// <returns>Collection of CaseModel objects.</returns>
        public List<CaseModel> GetCases(bool activeOnly = true, bool excludeClosed = true, bool resetCache = false)
        {
            // Get model from cache
            var cacheKey = string.Format("LCMS-Cases-{0}", !activeOnly ? "IncludingInactive" : string.Empty);
            var models = _memoryCache.Get(cacheKey) as List<CaseModel>;
            if (models == null || resetCache)
            {
                IQueryable<CaseView>? queryableEntities;
                var entities = new List<CaseView>();

                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    // Check for active
                    queryableEntities = activeOnly
                        ? dbContext.CaseViews.Where(x => x.IsActive)
                        : dbContext.CaseViews;

                    // Check for closed
                    if (excludeClosed)
                    {
                        queryableEntities = queryableEntities.Where(x => x.StatusId != (int)CaseStatus.Closed);
                    }

                    entities = queryableEntities.ToList();
                }

                // Convert to models
                models = new List<CaseModel>();
                foreach (var entity in entities)
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
                .OrderBy(x => x.CreatedDate)
                .ToList();
        }

        /// <summary>
        /// Save a Case.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool UpdateCase(CaseModel model, int userId)
        {
            var dbUpdated = false;
            Case entityBefore = new();
            Case entityAfter = new();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entityBefore = dbContext.Cases.Find(model.Id) ?? new();
                if (entityBefore.CaseId != 0)
                {
                    // Check for a name change
                    if (entityBefore.CaseTitle.ToLower().Trim() != model.Title.ToLower().Trim())
                    {
                        // Check for unique title
                        var titleIsUnique = CheckForUniqueTitle(model.Id, model.Title);
                        if (!titleIsUnique)
                        {
                            throw new ApplicationException("Case Title already exists - must be unique.");
                        }
                    }

                    // Update entity property values
                    entityAfter = entityBefore;
                    entityAfter.CaseTypeId = (int)model.Type;
                    entityAfter.CaseStatusId = (int)model.Status;
                    entityAfter.CaseIsActive = model.IsActive;
                    entityAfter.CaseTitle = model.Title.Trim();
                    entityAfter.CaseSummary = model.Summary?.Trim();

                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.UpdateCase(entityBefore, entityAfter, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Delete a Case from the database.
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteCase(int caseId, int userId)
        {
            var dbUpdated = false;
            Case entityBefore = new();
            Case entityAfter = new();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                // Get entity
                entityBefore = dbContext.Cases.Find(caseId) ?? new();
                if (entityBefore.CaseId != 0)
                {
                    // Update entity property values
                    entityAfter = entityBefore;
                    entityAfter.CaseIsDeleted = true;

                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteCase(entityBefore, entityAfter, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Check if the title is unique.
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="title"></param>
        /// <returns><c>true</c> if unique, otherwise <c>false</c>.</returns>
        public bool CheckForUniqueTitle(int caseId, string title)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return !dbContext.Cases.Any(x => x.CaseClientId != caseId && x.CaseTitle.ToLower() == title.ToLower().Trim());
        }

        /// <summary>
        /// Get Case key/value pairs.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <param name="excludeClosed"></param>
        /// <returns>Collection of key/value pairs.</returns>
        public List<KeyValuePair<int, string>> GetCaseKeyValuePairs(bool activeOnly = true, bool excludeClosed = true)
        {
            var models = GetCases(activeOnly, excludeClosed);

            var keyValuePairs = new List<KeyValuePair<int, string>>();
            foreach (var model in models)
            {
                keyValuePairs.Add(new KeyValuePair<int, string>(model.Id, model.Title));
            }

            return keyValuePairs;
        }

        /// <summary>
        /// Get Case Comment.
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<CommentModel> GetCaseComments(int caseId)
        {
            // Get entities
            List<CaseCommentView> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.CaseCommentViews.Where(x => x.CaseId == caseId).ToList();
            }

            // Convert to models
            var models = new List<CommentModel>();
            foreach (var entity in entities.OrderBy(x => x.CaseId))
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
        /// Add a Comment to a Case.
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateCaseComment(int caseId, string comment, int userId)
        {
            var dbUpdated = false;
            CaseComment? entity;

            // Check for existing Case user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = new CaseComment
                {
                    CaseCommentCaseId = caseId,
                    CaseCommentBody = comment,
                };

                dbContext.CaseComments.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateCaseComment(entity, userId);
            }

            return true;
        }

        /// <summary>
        /// Delete a Comment from a Case.
        /// </summary>
        /// <param name="caseCommentId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteCaseComment(int caseCommentId, int userId)
        {
            var dbUpdated = false;
            CaseComment entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.CaseComments.Find(caseCommentId) ?? new();
                if (entity.CaseCommentId != 0)
                {
                    entity.CaseCommentIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteCaseComment(entity, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Get Case Documents.
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<DocumentModel> GetCaseDocuments(int caseId)
        {
            // Get entities
            List<CaseDocumentView> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.CaseDocumentViews.Where(x => x.CaseId == caseId).ToList();
            }

            // Convert to models
            var models = new List<DocumentModel>();
            foreach (var entity in entities.OrderBy(x => x.CaseId))
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
        /// Add a Document to a Case.
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="model"></param>
        /// <param name="userId_Source"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateCaseDocument(int caseId, DocumentModel model, int userId_Source)
        {
            var dbUpdated = false;
            CaseDocument? entity;

            // Check for existing Case user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = new CaseDocument
                {
                    CaseDocumentCaseId = caseId,
                    CaseDocumentTypeId = (int)model.Type,
                    CaseDocumentTitle = model.Title,
                    CaseDocumentOriginalFileName = model.OriginalFileName,
                };

                dbContext.CaseDocuments.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateCaseDocument(entity, userId_Source);
            }

            return true;
        }

        /// <summary>
        /// Delete a Document from a Case.
        /// </summary>
        /// <param name="caseDocumentId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteCaseDocument(int caseDocumentId, int userId)
        {
            var dbUpdated = false;
            CaseDocument entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.CaseDocuments.Find(caseDocumentId) ?? new();
                if (entity.CaseDocumentId != 0)
                {
                    entity.CaseDocumentIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteCaseDocument(entity, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Get Case Notes.
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<NoteModel> GetCaseNotes(int caseId)
        {
            // Get entities
            List<CaseNoteView> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.CaseNoteViews.Where(x => x.CaseId == caseId).ToList();
            }

            // Convert to models
            var models = new List<NoteModel>();
            foreach (var entity in entities.OrderBy(x => x.CaseId))
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
        /// Add a Note to a Case.
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="note"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateCaseNote(int caseId, string note, int userId)
        {
            var dbUpdated = false;
            CaseNote? entity;

            // Check for existing Case user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = new CaseNote
                {
                    CaseNoteCaseId = caseId,
                    CaseNoteBody = note,
                };

                dbContext.CaseNotes.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateCaseNote(entity, userId);
            }

            return true;
        }

        /// <summary>
        /// Delete a Note from a Case.
        /// </summary>
        /// <param name="caseNoteId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteCaseNote(int caseNoteId, int userId)
        {
            var dbUpdated = false;
            CaseNote entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.CaseNotes.Find(caseNoteId) ?? new();
                if (entity.CaseNoteId != 0)
                {
                    entity.CaseNoteIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteCaseNote(entity, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Get Case users.
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<UserModel> GetCaseUsers(int caseId)
        {
            // Get entities
            List<UserView> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.CaseUserViews
                    .Join(dbContext.UserViews,
                        CaseUserView => CaseUserView.UserId,
                        userView => userView.UserId,
                        (CaseUserViews, userView) => new { CaseUserViews = CaseUserViews, UserView = userView })
                    .Where(x => x.CaseUserViews.CaseId == caseId)
                    .Select(x => x.UserView)
                    .ToList();
            }

            // Convert to models
            var models = new List<UserModel>();
            foreach (var entity in entities.OrderBy(x => x.LastName).ThenBy(x => x.FirstName))
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
        /// Add a user to a Case.
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="userId"></param>
        /// <param name="userId_Source"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateCaseUser(int caseId, int userId, int userId_Source)
        {
            var dbUpdated = false;
            CaseUser? entity;

            // Check for existing Case user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.CaseUsers.FirstOrDefault(x => x.CaseUserCaseId == caseId && x.CaseUserUserId == userId);
                if (entity == null)
                {
                    entity = new CaseUser
                    {
                        CaseUserCaseId = caseId,
                        CaseUserUserId = userId,
                    };

                    dbContext.CaseUsers.Add(entity);
                }
                else
                {
                    entity.CaseUserIsDeleted = false;
                }

                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateCaseUser(entity, userId_Source);
            }

            return true;
        }

        /// <summary>
        /// Delete a user from a Case.
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="userId"></param>
        /// <param name="userId_Source"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteCaseUser(int caseId, int userId, int userId_Source)
        {
            var dbUpdated = false;
            CaseUser entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.CaseUsers.FirstOrDefault(x => x.CaseUserCaseId == caseId && x.CaseUserUserId == userId) ?? new();
                if (entity.CaseUserId != 0)
                {
                    entity.CaseUserIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteCaseUser(entity, userId_Source);
            }

            return dbUpdated;
        }

        #endregion

        #region Private Methods

        private static CaseModel? GetModel(Case? entity)
        {
            if (entity == null) return null;

            var model = new CaseModel
            {
                Id = entity.CaseId,
                Guid = entity.CaseGuid,
                ClientId = entity.CaseClientId,
                TypeId = entity.CaseTypeId,
                StatusId = entity.CaseStatusId,
                IsActive = entity.CaseIsActive,
                IsDeleted = entity.CaseIsDeleted,
                Title = entity.CaseTitle,
                Summary = entity.CaseSummary,
            };

            return model;
        }

        internal static CaseModel? GetModel(CaseView? entity)
        {
            if (entity == null) return null;

            var model = new CaseModel
            {
                Id = entity.CaseId,
                Guid = entity.Guid,
                ClientId = entity.ClientId,
                TypeId = entity.TypeId,
                StatusId = entity.StatusId,
                IsActive = entity.IsActive,
                Title = entity.Title,
                Summary = entity.Summary,
                ClientName = entity.ClientName,
                //CreatedDate = entity.CreatedDate,
                //CreatedBy = entity.CreatedBy,
                //ModifiedDate = entity.ModifiedDate,
                //ModifiedBy = entity.ModifiedBy,
            };

            return model;
        }

        private static CommentModel? GetModel(CaseCommentView entity)
        {
            if (entity == null) return null;

            var model = new CommentModel
            {
                Id = entity.CaseCommentId,
                Comment = entity.Comment,
                //CreatedDate = entity.CreatedDate,
                //CreatedBy = entity.CreatedBy,
                //ModifiedDate = entity.ModifiedDate,
                //ModifiedBy = entity.ModifiedBy,
            };

            return model;
        }

        private static DocumentModel? GetModel(CaseDocumentView entity)
        {
            if (entity == null) return null;

            var model = new DocumentModel
            {
                Id = entity.CaseDocumentId,
                TypeId = entity.CaseDocumentTypeId,
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

        private static NoteModel? GetModel(CaseNoteView entity)
        {
            if (entity == null) return null;

            var model = new NoteModel
            {
                Id = entity.CaseNoteId,
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
