using System.Diagnostics.Contracts;
using LCMS.Models;
using LCMS.Repository.Entities;
using LCMS.Services.BaseClasses;
using LCMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static LCMS.Models.Enums;
using Contract = LCMS.Repository.Entities.Contract;

namespace LCMS.Services
{
    public class ContractService(IDbContextFactory<LCMSDatabaseContext> dbContextFactory, IMemoryCache memoryCache, IServiceProvider serviceProvider, IConfiguration configuration)
        : DbContextService(dbContextFactory, memoryCache, serviceProvider, configuration), IContractService
    {
        #region Public Methods

        /// <summary>
        /// Add a new Contract to the database.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns>New ContractModel object.</returns>
        public ContractModel? CreateContract(ContractModel model, int userId)
        {
            var entity = new Contract
            {
                ContractClientId = model.ClientId,
                ContractTypeId = (int)model.ContractType,
                ContractStatusId = (int)model.Status,
                ContractTitle = model.Title.Trim(),
                ContractSummary = model.Summary?.Trim(),
                ContractIsActive = model.IsActive,
            };

            bool dbUpdated;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                dbContext.Contracts.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateContract(entity, userId);

                // Update cache
                GetContracts(true, true, true);
            }

            return GetModel(entity);
        }

        /// <summary>
        /// Get Contract.
        /// </summary>
        /// <param name="contractId"></param>
        /// <returns>ContractModel object.</returns>
        public ContractModel? GetContract(int contractId)
        {
            Contract? entity = null;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.Contracts.Find(contractId);
            }

            var model = GetModel(entity);
            if (model != null)
            {
                // HACK: Manually populate ClientName
                using var scope = _serviceProvider.CreateScope();
                var clientService = scope.ServiceProvider.GetRequiredService<IClientService>();
                var clients = clientService.GetClientKeyValuePairs(false, false);
                model.ClientName = clients.FirstOrDefault(x => x.Key == model.ClientId).Value;
            }

            return model;
        }

        /// <summary>
        /// Get Contracts.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <param name="excludeClosed"></param>
        /// <param name="resetCache"></param>
        /// <returns>Collection of ContractModel objects.</returns>
        public List<ContractModel> GetContracts(bool activeOnly = true, bool excludeClosed = true, bool resetCache = false)
        {
            // Get model from cache
            var cacheKey = string.Format("LCMS-Contracts-{0}", !activeOnly ? "IncludingInactive" : string.Empty);
            var models = _memoryCache.Get(cacheKey) as List<ContractModel>;
            if (models == null || resetCache)
            {
                var entities = new List<Contract>();

                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    // Check for active
                    entities = activeOnly
                        ? dbContext.Contracts.Where(x => !x.ContractIsDeleted && x.ContractIsActive).ToList()
                        : dbContext.Contracts.Where(x => !x.ContractIsDeleted).ToList();

                    // Check for closed
                    if (excludeClosed)
                    {
                        entities = entities.Where(x => x.ContractStatusId != (int)ContractStatus.Closed).ToList();
                    }
                }

                // HACK: ClientView isn't working due to NULL value mapping,
                // so for now we'll have to manually populate ClientName
                using var scope = _serviceProvider.CreateScope();
                var clientService = scope.ServiceProvider.GetRequiredService<IClientService>();
                var clients = clientService.GetClientKeyValuePairs(false, false);

                // Convert to models
                models = new List<ContractModel>();
                foreach (var entity in entities)
                {
                    var model = GetModel(entity);
                    if (model != null)
                    {
                        model.ClientName = clients.FirstOrDefault(x => x.Key == model.ClientId).Value;
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
        /// Save a Contract.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool UpdateContract(ContractModel model, int userId)
        {
            var dbUpdated = false;
            Contract entityBefore = new();
            Contract entityAfter = new();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entityBefore = dbContext.Contracts.Find(model.Id) ?? new();
                if (entityBefore.ContractId != 0)
                {
                    // Check for a name change
                    if (entityBefore.ContractTitle.ToLower().Trim() != model.Title.ToLower().Trim())
                    {
                        // Check for unique title
                        var titleIsUnique = CheckForUniqueTitle(model.Id, model.Title);
                        if (!titleIsUnique)
                        {
                            throw new ApplicationException("Contract Title already exists - must be unique.");
                        }
                    }

                    // Update entity property values
                    entityAfter = entityBefore;
                    entityAfter.ContractTypeId = (int)model.ContractType;
                    entityAfter.ContractStatusId = (int)model.Status;
                    entityAfter.ContractIsActive = model.IsActive;
                    entityAfter.ContractTitle = model.Title.Trim();
                    entityAfter.ContractSummary = model.Summary?.Trim();

                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.UpdateContract(entityBefore, entityAfter, userId);

                // Update cache
                GetContracts(true, true, true);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Delete a Contract from the database.
        /// </summary>
        /// <param name="contractId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteContract(int contractId, int userId)
        {
            var dbUpdated = false;
            Contract entityBefore = new();
            Contract entityAfter = new();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                // Get entity
                entityBefore = dbContext.Contracts.Find(contractId) ?? new();
                if (entityBefore.ContractId != 0)
                {
                    // Update entity property values
                    entityAfter = entityBefore;
                    entityAfter.ContractIsDeleted = true;

                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteContract(entityBefore, entityAfter, userId);

                // Update cache
                GetContracts(true, true, true);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Check if the title is unique.
        /// </summary>
        /// <param name="contractId"></param>
        /// <param name="title"></param>
        /// <returns><c>true</c> if unique, otherwise <c>false</c>.</returns>
        public bool CheckForUniqueTitle(int contractId, string title)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return !dbContext.Contracts.Any(x => x.ContractClientId != contractId && x.ContractTitle.ToLower() == title.ToLower().Trim());
        }

        /// <summary>
        /// Get Contract key/value pairs.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <param name="excludeClosed"></param>
        /// <returns>Collection of key/value pairs.</returns>
        public List<KeyValuePair<int, string>> GetContractKeyValuePairs(bool activeOnly = true, bool excludeClosed = true)
        {
            var models = GetContracts(activeOnly, excludeClosed);

            var keyValuePairs = new List<KeyValuePair<int, string>>();
            foreach (var model in models)
            {
                keyValuePairs.Add(new KeyValuePair<int, string>(model.Id, model.Title));
            }

            return keyValuePairs;
        }

        /// <summary>
        /// Get Contract Comment.
        /// </summary>
        /// <param name="contractId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<CommentModel> GetContractComments(int contractId)
        {
            // Get entities
            List<ContractCommentView> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.ContractCommentViews.Where(x => x.ContractId == contractId).ToList();
            }

            // Convert to models
            var models = new List<CommentModel>();
            foreach (var entity in entities.OrderBy(x => x.ContractId))
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
        /// Add a Comment to a Contract.
        /// </summary>
        /// <param name="contractId"></param>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateContractComment(int contractId, string comment, int userId)
        {
            var dbUpdated = false;
            ContractComment? entity;

            // Check for existing Contract user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = new ContractComment
                {
                    ContractCommentContractId = contractId,
                    ContractCommentBody = comment,
                };

                dbContext.ContractComments.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateContractComment(entity, userId);
            }

            return true;
        }

        /// <summary>
        /// Delete a Comment from a Contract.
        /// </summary>
        /// <param name="contractCommentId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteContractComment(int contractCommentId, int userId)
        {
            var dbUpdated = false;
            ContractComment entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.ContractComments.Find(contractCommentId) ?? new();
                if (entity.ContractCommentId != 0)
                {
                    entity.ContractCommentIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteContractComment(entity, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Get Contract Documents.
        /// </summary>
        /// <param name="contractId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<DocumentModel> GetContractDocuments(int contractId)
        {
            // Get entities
            List<ContractDocument> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.ContractDocuments.Where(x => x.ContractDocumentContractId == contractId && !x.ContractDocumentIsDeleted).ToList();
            }

            // Convert to models
            var models = new List<DocumentModel>();
            foreach (var entity in entities.OrderBy(x => x.ContractDocumentId))
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
        /// Add a Document to a Contract.
        /// </summary>
        /// <param name="contractId"></param>
        /// <param name="model"></param>
        /// <param name="userId_Source"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateContractDocument(int contractId, DocumentModel model, int userId_Source)
        {
            var dbUpdated = false;
            ContractDocument? entity;

            // Check for existing Contract user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = new ContractDocument
                {
                    ContractDocumentContractId = contractId,
                    ContractDocumentTypeId = (int)model.Type,
                    ContractDocumentTitle = model.Title,
                    ContractDocumentOriginalFileName = model.OriginalFileName,
                };

                dbContext.ContractDocuments.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateContractDocument(entity, userId_Source);
            }

            return true;
        }

        /// <summary>
        /// Delete a Document from a Contract.
        /// </summary>
        /// <param name="contractDocumentId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteContractDocument(int contractDocumentId, int userId)
        {
            var dbUpdated = false;
            ContractDocument entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.ContractDocuments.Find(contractDocumentId) ?? new();
                if (entity.ContractDocumentId != 0)
                {
                    entity.ContractDocumentIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteContractDocument(entity, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Get Contract Notes.
        /// </summary>
        /// <param name="contractId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<NoteModel> GetContractNotes(int contractId)
        {
            // Get entities
            List<ContractNoteView> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.ContractNoteViews.Where(x => x.ContractId == contractId).ToList();
            }

            // Convert to models
            var models = new List<NoteModel>();
            foreach (var entity in entities.OrderBy(x => x.ContractId))
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
        /// Add a Note to a Contract.
        /// </summary>
        /// <param name="contractId"></param>
        /// <param name="note"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateContractNote(int contractId, string note, int userId)
        {
            var dbUpdated = false;
            ContractNote? entity;

            // Check for existing Contract user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = new ContractNote
                {
                    ContractNoteContractId = contractId,
                    ContractNoteBody = note,
                };

                dbContext.ContractNotes.Add(entity);
                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateContractNote(entity, userId);
            }

            return true;
        }

        /// <summary>
        /// Delete a Note from a Contract.
        /// </summary>
        /// <param name="contractNoteId"></param>
        /// <param name="userId"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteContractNote(int contractNoteId, int userId)
        {
            var dbUpdated = false;
            ContractNote entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.ContractNotes.Find(contractNoteId) ?? new();
                if (entity.ContractNoteId != 0)
                {
                    entity.ContractNoteIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteContractNote(entity, userId);
            }

            return dbUpdated;
        }

        /// <summary>
        /// Get Contract users.
        /// </summary>
        /// <param name="contractId"></param>
        /// <returns>Collection of UserModel objects.</returns>
        public List<UserModel> GetContractUsers(int contractId)
        {
            // Get entities
            List<User> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.ContractUsers
                    .Join(dbContext.Users,
                        contractUser => contractUser.ContractUserUserId,
                        user => user.UserId,
                        (contractUsers, users) => new { ContractUsers = contractUsers, User = users })
                    .Where(x => !x.ContractUsers.ContractUserIsDeleted && x.ContractUsers.ContractUserContractId == contractId)
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
        /// Add a user to a Contract.
        /// </summary>
        /// <param name="contractId"></param>
        /// <param name="userId"></param>
        /// <param name="userId_Source"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool CreateContractUser(int contractId, int userId, int userId_Source)
        {
            var dbUpdated = false;
            ContractUser? entity;

            // Check for existing Contract user
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.ContractUsers.FirstOrDefault(x => x.ContractUserContractId == contractId && x.ContractUserUserId == userId);
                if (entity == null)
                {
                    entity = new ContractUser
                    {
                        ContractUserContractId = contractId,
                        ContractUserUserId = userId,
                    };

                    dbContext.ContractUsers.Add(entity);
                }
                else
                {
                    entity.ContractUserIsDeleted = false;
                }

                dbUpdated = dbContext.SaveChanges() > 0;
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.CreateContractUser(entity, userId_Source);
            }

            return true;
        }

        /// <summary>
        /// Delete a user from a Contract.
        /// </summary>
        /// <param name="contractId"></param>
        /// <param name="userId"></param>
        /// <param name="userId_Source"></param>
        /// <returns><c>true</c> if successful, otherwise <c>fale</c>.</returns>
        public bool DeleteContractUser(int contractId, int userId, int userId_Source)
        {
            var dbUpdated = false;
            ContractUser entity;

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity = dbContext.ContractUsers.FirstOrDefault(x => x.ContractUserContractId == contractId && x.ContractUserUserId == userId) ?? new();
                if (entity.ContractUserId != 0)
                {
                    entity.ContractUserIsDeleted = true;
                    dbUpdated = dbContext.SaveChanges() > 0;
                }
            }

            if (dbUpdated)
            {
                // Create audit record
                using var scope = _serviceProvider.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                auditService.DeleteContractUser(entity, userId_Source);
            }

            return dbUpdated;
        }

        #endregion

        #region Private Methods

        private static ContractModel? GetModel(Contract? entity)
        {
            if (entity == null) return null;

            var model = new ContractModel
            {
                Id = entity.ContractId,
                Guid = entity.ContractGuid,
                ClientId = entity.ContractClientId,
                ContractTypeId = entity.ContractTypeId,
                StatusId = entity.ContractStatusId,
                IsActive = entity.ContractIsActive,
                IsDeleted = entity.ContractIsDeleted,
                Title = entity.ContractTitle,
                Summary = entity.ContractSummary,
            };

            return model;
        }

        internal static ContractModel? GetModel(ContractView? entity)
        {
            if (entity == null) return null;

            var model = new ContractModel
            {
                Id = entity.ContractId,
                Guid = entity.Guid,
                ClientId = entity.ClientId,
                ContractTypeId = entity.TypeId,
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

        private static CommentModel? GetModel(ContractCommentView entity)
        {
            if (entity == null) return null;

            var model = new CommentModel
            {
                Id = entity.ContractCommentId,
                Comment = entity.Comment,
                //CreatedDate = entity.CreatedDate,
                //CreatedBy = entity.CreatedBy,
                //ModifiedDate = entity.ModifiedDate,
                //ModifiedBy = entity.ModifiedBy,
            };

            return model;
        }

        private static DocumentModel? GetModel(ContractDocument entity)
        {
            if (entity == null) return null;

            var model = new DocumentModel
            {
                Id = entity.ContractDocumentId,
                TypeId = entity.ContractDocumentTypeId,
                Title = entity.ContractDocumentTitle,
                Summary = entity.ContractDocumentSummary,
                OriginalFileName = entity.ContractDocumentOriginalFileName,
                //CreatedDate = entity.CreatedDate,
                //CreatedBy = entity.CreatedBy,
                //ModifiedDate = entity.ModifiedDate,
                //ModifiedBy = entity.ModifiedBy,
            };

            return model;
        }

        private static DocumentModel? GetModel(ContractDocumentView entity)
        {
            if (entity == null) return null;

            var model = new DocumentModel
            {
                Id = entity.ContractDocumentId,
                TypeId = entity.ContractDocumentTypeId,
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

        private static NoteModel? GetModel(ContractNoteView entity)
        {
            if (entity == null) return null;

            var model = new NoteModel
            {
                Id = entity.ContractNoteId,
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
