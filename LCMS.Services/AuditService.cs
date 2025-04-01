using LCMS.Models;
using LCMS.Repository.Entities;
using LCMS.Services.BaseClasses;
using LCMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection;
using static LCMS.Models.Enums;
using DataDictionaryGroup = LCMS.Repository.Entities.DataDictionaryGroup;

namespace LCMS.Services
{
    public class AuditService(IDbContextFactory<LCMSDatabaseContext> dbContextFactory, IMemoryCache memoryCache, IServiceProvider serviceProvider, IConfiguration configuration)
        : DbContextService(dbContextFactory, memoryCache, serviceProvider, configuration), IAuditService
    {
        #region Public Methods

        public List<AuditModel> GetDataDictionaryGroupAudits(int dataDictionaryGroupId)
        {
            var models = new List<AuditModel>();

            // Get entities
            var entities = _dbContextFactory.CreateDbContext().DataDictionaryGroupAudits.Where(a => a.DataDictionaryGroupAuditDataDictionaryGroupId == dataDictionaryGroupId);

            // Convert to models
            foreach (var entity in entities)
            {
                models.Add(GetModel(entity));
            }

            return models;
        }

        public bool CreateDataDictionaryGroup(DataDictionaryGroup entity, int userId)
        {
            var entityBefore = new DataDictionaryGroup();
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entity);
            var affectedColumns = GetAffectedColumns(entityBefore, entity);
            return CreateDataDictionaryGroupAudit(entity.DataDictionaryGroupId, userId, AuditAction.Create, beforeJson, afterJson, affectedColumns);
        }

        public bool UpdateDataDictionaryGroup(DataDictionaryGroup entityBefore, DataDictionaryGroup entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateDataDictionaryGroupAudit(entityAfter.DataDictionaryGroupId, userId, AuditAction.Update, beforeJson, afterJson, affectedColumns);
        }

        public bool DeleteDataDictionaryGroup(DataDictionaryGroup entityBefore, DataDictionaryGroup entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateDataDictionaryGroupAudit(entityAfter.DataDictionaryGroupId, userId, AuditAction.Delete, beforeJson, afterJson, affectedColumns);
        }

        public List<AuditModel> GetDataDictionaryAudits(int dataDictionaryId)
        {
            var models = new List<AuditModel>();

            // Get entities
            var entities = _dbContextFactory.CreateDbContext().DataDictionaryAudits.Where(a => a.DataDictionaryAuditDataDictionaryId == dataDictionaryId);

            // Convert to models
            foreach (var entity in entities)
            {
                models.Add(GetModel(entity));
            }

            return models;
        }

        public bool CreateDataDictionary(DataDictionary entity, int userId)
        {
            var entityBefore = new DataDictionary();
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entity);
            var affectedColumns = GetAffectedColumns(entityBefore, entity);
            return CreateDataDictionaryAudit(entity.DataDictionaryId, userId, AuditAction.Create, beforeJson, afterJson, affectedColumns);
        }

        public bool UpdateDataDictionary(DataDictionary entityBefore, DataDictionary entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateDataDictionaryAudit(entityAfter.DataDictionaryId, userId, AuditAction.Update, beforeJson, afterJson, affectedColumns);
        }

        public bool DeleteDataDictionary(DataDictionary entityBefore, DataDictionary entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateDataDictionaryAudit(entityAfter.DataDictionaryId, userId, AuditAction.Delete, beforeJson, afterJson, affectedColumns);
        }

        public List<AuditModel> GetUserAccountAudits(int userAccountId)
        {
            var models = new List<AuditModel>();

            // Get entities
            var entities = _dbContextFactory.CreateDbContext().UserAccountAudits.Where(a => a.UserAccountAuditUserAccountId == userAccountId);

            // Convert to models
            foreach (var entity in entities)
            {
                models.Add(GetModel(entity));
            }

            return models;
        }

        public bool CreateUserAccount(UserAccount entity, int userId)
        {
            var entityBefore = new UserAccount();
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entity);
            var affectedColumns = GetAffectedColumns(entityBefore, entity);
            return CreateUserAccountAudit(entity.UserAccountId, userId, AuditAction.Create, beforeJson, afterJson, affectedColumns);
        }

        public bool UpdateUserAccount(UserAccount entityBefore, UserAccount entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateUserAccountAudit(entityAfter.UserAccountId, userId, AuditAction.Update, beforeJson, afterJson, affectedColumns);
        }

        public bool DeleteUserAccount(UserAccount entityBefore, UserAccount entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateUserAccountAudit(entityAfter.UserAccountId, userId, AuditAction.Delete, beforeJson, afterJson, affectedColumns);
        }

        public List<AuditModel> GetClientAudits(int clientId)
        {
            var models = new List<AuditModel>();

            // Get entities
            var entities = _dbContextFactory.CreateDbContext().ClientAudits.Where(a => a.ClientAuditClientId == clientId);

            // Convert to models
            foreach (var entity in entities)
            {
                models.Add(GetModel(entity));
            }

            return models;
        }

        public bool CreateClient(Client entity, int userId)
        {
            var entityBefore = new Client();
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entity);
            var affectedColumns = GetAffectedColumns(entityBefore, entity);
            return CreateClientAudit(entity.ClientId, userId, AuditAction.Create, beforeJson, afterJson, affectedColumns);
        }

        public bool UpdateClient(Client entityBefore, Client entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateClientAudit(entityAfter.ClientId, userId, AuditAction.Update, beforeJson, afterJson, affectedColumns);
        }

        public bool DeleteClient(Client entityBefore, Client entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateClientAudit(entityAfter.ClientId, userId, AuditAction.Delete, beforeJson, afterJson, affectedColumns);
        }

        public List<AuditModel> GetUserAudits(int userId)
        {
            var models = new List<AuditModel>();

            // Get entities
            List<UserAudit> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.UserAudits.Where(a => a.UserAuditUserId == userId).ToList();
            }

            // Convert to models
            foreach (var entity in entities)
            {
                models.Add(GetModel(entity));
            }

            return models;
        }

        public bool CreateUser(User entity, int userId)
        {
            var entityBefore = new User();
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entity);
            var affectedColumns = GetAffectedColumns(entityBefore, entity);
            return CreateUserAudit(entity.UserId, userId, AuditAction.Create, beforeJson, afterJson, affectedColumns);
        }

        public bool UpdateUser(User entityBefore, User entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateUserAudit(entityAfter.UserId, userId, AuditAction.Update, beforeJson, afterJson, affectedColumns);
        }

        public bool DeleteUser(User entityBefore, User entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateUserAudit(entityAfter.UserId, userId, AuditAction.Delete, beforeJson, afterJson, affectedColumns);
        }

        public List<AuditModel> GetCaseAudits(int caseId)
        {
            var models = new List<AuditModel>();

            // Get entities
            List<CaseAudit> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.CaseAudits.Where(a => a.CaseAuditCaseId == caseId).ToList();
            }

            // Convert to models
            foreach (var entity in entities)
            {
                models.Add(GetModel(entity));
            }

            return models;
        }

        public bool CreateCase(Case entity, int userId)
        {
            var entityBefore = new Case();
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entity);
            var affectedColumns = GetAffectedColumns(entityBefore, entity);
            return CreateCaseAudit(entity.CaseId, userId, AuditAction.Create, beforeJson, afterJson, affectedColumns);
        }

        public bool UpdateCase(Case entityBefore, Case entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateCaseAudit(entityAfter.CaseId, userId, AuditAction.Update, beforeJson, afterJson, affectedColumns);
        }

        public bool DeleteCase(Case entityBefore, Case entityAfter, int userId)
        {
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            var affectedColumns = GetAffectedColumns(entityBefore, entityAfter);
            return CreateCaseAudit(entityAfter.CaseId, userId, AuditAction.Delete, beforeJson, afterJson, affectedColumns);
        }

        public List<AuditModel> GetCaseCommentAudits(int caseCommentId)
        {
            var models = new List<AuditModel>();

            // Get entities
            List<CaseCommentAudit> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.CaseCommentAudits.Where(a => a.CaseCommentAuditCaseCommentId == caseCommentId).ToList();
            }

            // Convert to models
            foreach (var entity in entities)
            {
                models.Add(GetModel(entity));
            }

            return models;
        }

        public bool CreateCaseComment(CaseComment entity, int userId)
        {
            var entityBefore = new CaseComment();
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entity);
            return CreateCaseCommentAudit(entity.CaseCommentId, userId, AuditAction.Create, beforeJson, afterJson);
        }

        public bool DeleteCaseComment(CaseComment entity, int userId)
        {
            var entityAfter = new CaseComment();
            var beforeJson = JsonConvert.SerializeObject(entity);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            return CreateCaseCommentAudit(entity.CaseCommentId, userId, AuditAction.Delete, beforeJson, afterJson);
        }

        public List<AuditModel> GetCaseDocumentAudits(int caseDocumentId)
        {
            var models = new List<AuditModel>();

            // Get entities
            List<CaseDocumentAudit> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.CaseDocumentAudits.Where(a => a.CaseDocumentAuditCaseDocumentId == caseDocumentId).ToList();
            }

            // Convert to models
            foreach (var entity in entities)
            {
                models.Add(GetModel(entity));
            }

            return models;
        }

        public bool CreateCaseDocument(CaseDocument entity, int userId)
        {
            var entityBefore = new CaseDocument();
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entity);
            return CreateCaseDocumentAudit(entity.CaseDocumentId, userId, AuditAction.Create, beforeJson, afterJson);
        }

        public bool DeleteCaseDocument(CaseDocument entity, int userId)
        {
            var entityAfter = new CaseDocument();
            var beforeJson = JsonConvert.SerializeObject(entity);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            return CreateCaseDocumentAudit(entity.CaseDocumentId, userId, AuditAction.Delete, beforeJson, afterJson);
        }

        public List<AuditModel> GetCaseNoteAudits(int caseNoteId)
        {
            var models = new List<AuditModel>();

            // Get entities
            List<CaseNoteAudit> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.CaseNoteAudits.Where(a => a.CaseNoteAuditCaseNoteId == caseNoteId).ToList();
            }

            // Convert to models
            foreach (var entity in entities)
            {
                models.Add(GetModel(entity));
            }

            return models;
        }

        public bool CreateCaseNote(CaseNote entity, int userId)
        {
            var entityBefore = new CaseNote();
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entity);
            return CreateCaseNoteAudit(entity.CaseNoteId, userId, AuditAction.Create, beforeJson, afterJson);
        }

        public bool DeleteCaseNote(CaseNote entity, int userId)
        {
            var entityAfter = new CaseNote();
            var beforeJson = JsonConvert.SerializeObject(entity);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            return CreateCaseNoteAudit(entity.CaseNoteId, userId, AuditAction.Delete, beforeJson, afterJson);
        }

        public List<AuditModel> GetCaseUserAudits(int caseUserId)
        {
            var models = new List<AuditModel>();

            // Get entities
            List<CaseUserAudit> entities;
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entities = dbContext.CaseUserAudits.Where(a => a.CaseUserAuditCaseUserId == caseUserId).ToList();
            }

            // Convert to models
            foreach (var entity in entities)
            {
                models.Add(GetModel(entity));
            }

            return models;
        }

        public bool CreateCaseUser(CaseUser entity, int userId)
        {
            var entityBefore = new CaseUser();
            var beforeJson = JsonConvert.SerializeObject(entityBefore);
            var afterJson = JsonConvert.SerializeObject(entity);
            return CreateCaseUserAudit(entity.CaseUserId, userId, AuditAction.Create, beforeJson, afterJson);
        }

        public bool DeleteCaseUser(CaseUser entity, int userId)
        {
            var entityAfter = new CaseUser();
            var beforeJson = JsonConvert.SerializeObject(entity);
            var afterJson = JsonConvert.SerializeObject(entityAfter);
            return CreateCaseUserAudit(entity.CaseUserId, userId, AuditAction.Delete, beforeJson, afterJson);
        }

        #endregion

        #region Private Methods

        private static List<string> GetAffectedColumns(object before, object after)
        {
            // Get properties to compare
            var propertiesToCompare = before.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Loop through properties and create list of values that have changed
            var changedProperties = new List<string>();
            foreach (var propertyInfo in propertiesToCompare.Where(a => a.Name != "ExtendedFields"))
            {
                // Check to see if property value has changed, if so add to list
                if (ValueChangedBoolean(propertyInfo.GetValue(before, null), propertyInfo.GetValue(after, null)))
                {
                    changedProperties.Add(propertyInfo.Name);
                }
            }

            return changedProperties;
        }

        private static bool ValueChangedBoolean(object? beforePropertyValue, object? afterPropertyValue)
        {
            // Compare property values
            var propertyValueHasChanged = false;
            var selfValueComparer = beforePropertyValue as IComparable;
            if (beforePropertyValue == null && afterPropertyValue != null ||
                beforePropertyValue != null && afterPropertyValue == null)
            {
                propertyValueHasChanged = true; // one of the values is null
            }
            else if (selfValueComparer != null && selfValueComparer.CompareTo(afterPropertyValue) != 0)
            {
                propertyValueHasChanged = true; // the comparison using IComparable failed
            }
            else if (!Equals(beforePropertyValue, afterPropertyValue))
            {
                propertyValueHasChanged = true; // the comparison using Equals failed
            }

            // Check to see if property value has changed, if so add to list
            return propertyValueHasChanged;
        }

        private bool CreateDataDictionaryGroupAudit(int dataDictionaryGroupId, int userId, AuditAction action, string beforeJson, string afterJson, List<string> affectedColumns)
        {
            // Create audit record
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.DataDictionaryGroupAudits.Add(new DataDictionaryGroupAudit
            {
                DataDictionaryGroupAuditDataDictionaryGroupId = dataDictionaryGroupId,
                DataDictionaryGroupAuditUserId = userId,
                DataDictionaryGroupAuditDate = DateTime.Now,
                DataDictionaryGroupAuditActionId = (int)action,
                DataDictionaryGroupAuditBeforeJson = beforeJson,
                DataDictionaryGroupAuditAfterJson = afterJson,
                DataDictionaryGroupAuditAffectedColumns = string.Join(",", affectedColumns),
            });

            return dbContext.SaveChanges() > 0;
        }

        private bool CreateDataDictionaryAudit(int dataDictionaryId, int userId, AuditAction action, string beforeJson, string afterJson, List<string> affectedColumns)
        {
            // Create audit record
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.DataDictionaryAudits.Add(new DataDictionaryAudit
            {
                DataDictionaryAuditDataDictionaryId = dataDictionaryId,
                DataDictionaryAuditUserId = userId,
                DataDictionaryAuditDate = DateTime.Now,
                DataDictionaryAuditActionId = (int)action,
                DataDictionaryAuditBeforeJson = beforeJson,
                DataDictionaryAuditAfterJson = afterJson,
                DataDictionaryAuditAffectedColumns = string.Join(",", affectedColumns),
            });

            return dbContext.SaveChanges() > 0;
        }

        private bool CreateUserAccountAudit(int userAccountId, int userId, AuditAction action, string beforeJson, string afterJson, List<string> affectedColumns)
        {
            // Create audit record
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.UserAccountAudits.Add(new UserAccountAudit
            {
                UserAccountAuditUserAccountId = userAccountId,
                UserAccountAuditUserId = userId,
                UserAccountAuditDate = DateTime.Now,
                UserAccountAuditActionId = (int)action,
                UserAccountAuditBeforeJson = beforeJson,
                UserAccountAuditAfterJson = afterJson,
                UserAccountAuditAffectedColumns = string.Join(",", affectedColumns),
            });

            return dbContext.SaveChanges() > 0;
        }

        private bool CreateClientAudit(int clientId, int userId, AuditAction action, string beforeJson, string afterJson, List<string> affectedColumns)
        {
            // Create audit record
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.ClientAudits.Add(new ClientAudit
            {
                ClientAuditClientId = clientId,
                ClientAuditUserId = userId,
                ClientAuditDate = DateTime.Now,
                ClientAuditActionId = (int)action,
                ClientAuditBeforeJson = beforeJson,
                ClientAuditAfterJson = afterJson,
                ClientAuditAffectedColumns = string.Join(",", affectedColumns),
            });

            return dbContext.SaveChanges() > 0;
        }

        private bool CreateUserAudit(int userId, int userId_Source, AuditAction action, string beforeJson, string afterJson, List<string> affectedColumns)
        {
            // Create audit record
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.UserAudits.Add(new UserAudit
            {
                UserAuditUserId = userId,
                UserAuditUserIdSource = userId_Source,
                UserAuditDate = DateTime.Now,
                UserAuditActionId = (int)action,
                UserAuditBeforeJson = beforeJson,
                UserAuditAfterJson = afterJson,
                UserAuditAffectedColumns = string.Join(",", affectedColumns),
            });

            return dbContext.SaveChanges() > 0;
        }

        private bool CreateCaseAudit(int caseId, int userId, AuditAction action, string beforeJson, string afterJson, List<string> affectedColumns)
        {
            // Create audit record
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.CaseAudits.Add(new CaseAudit
            {
                CaseAuditCaseId = caseId,
                CaseAuditUserId = userId,
                CaseAuditDate = DateTime.Now,
                CaseAuditActionId = (int)action,
                CaseAuditBeforeJson = beforeJson,
                CaseAuditAfterJson = afterJson,
                CaseAuditAffectedColumns = string.Join(",", affectedColumns),
            });

            return dbContext.SaveChanges() > 0;
        }

        private bool CreateCaseCommentAudit(int caseCommentId, int userId, AuditAction action, string beforeJson, string afterJson)
        {
            // Create audit record
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.CaseCommentAudits.Add(new CaseCommentAudit
            {
                CaseCommentAuditCaseCommentId = caseCommentId,
                CaseCommentAuditUserId = userId,
                CaseCommentAuditDate = DateTime.Now,
                CaseCommentAuditActionId = (int)action,
                CaseCommentAuditBeforeJson = beforeJson,
                CaseCommentAuditAfterJson = afterJson,
                CaseCommentAuditAffectedColumns = string.Empty,
            });

            return dbContext.SaveChanges() > 0;
        }

        private bool CreateCaseDocumentAudit(int caseDocumentId, int userId, AuditAction action, string beforeJson, string afterJson)
        {
            // Create audit record
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.CaseDocumentAudits.Add(new CaseDocumentAudit
            {
                CaseDocumentAuditCaseDocumentId = caseDocumentId,
                CaseDocumentAuditUserId = userId,
                CaseDocumentAuditDate = DateTime.Now,
                CaseDocumentAuditActionId = (int)action,
                CaseDocumentAuditBeforeJson = beforeJson,
                CaseDocumentAuditAfterJson = afterJson,
                CaseDocumentAuditAffectedColumns = string.Empty,
            });

            return dbContext.SaveChanges() > 0;
        }

        private bool CreateCaseNoteAudit(int caseNoteId, int userId, AuditAction action, string beforeJson, string afterJson)
        {
            // Create audit record
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.CaseNoteAudits.Add(new CaseNoteAudit
            {
                CaseNoteAuditCaseNoteId = caseNoteId,
                CaseNoteAuditUserId = userId,
                CaseNoteAuditDate = DateTime.Now,
                CaseNoteAuditActionId = (int)action,
                CaseNoteAuditBeforeJson = beforeJson,
                CaseNoteAuditAfterJson = afterJson,
                CaseNoteAuditAffectedColumns = string.Empty,
            });

            return dbContext.SaveChanges() > 0;
        }

        private bool CreateCaseUserAudit(int caseUserId, int userId, AuditAction action, string beforeJson, string afterJson)
        {
            // Create audit record
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.CaseUserAudits.Add(new CaseUserAudit
            {
                CaseUserAuditCaseUserId = caseUserId,
                CaseUserAuditUserId = userId,
                CaseUserAuditDate = DateTime.Now,
                CaseUserAuditActionId = (int)action,
                CaseUserAuditBeforeJson = beforeJson,
                CaseUserAuditAfterJson = afterJson,
                CaseUserAuditAffectedColumns = string.Empty,
            });

            return dbContext.SaveChanges() > 0;
        }

        private static AuditModel GetModel(DataDictionaryGroupAudit entity)
        {
            return new AuditModel
            {
                Id = entity.DataDictionaryGroupAuditId,
                Date = entity.DataDictionaryGroupAuditDate,
                Action = (AuditAction)entity.DataDictionaryGroupAuditActionId,
                BeforeJson = entity.DataDictionaryGroupAuditBeforeJson,
                AfterJson = entity.DataDictionaryGroupAuditAfterJson,
                AffectedColumns = entity.DataDictionaryGroupAuditAffectedColumns.Split(',').ToList(),
            };
        }

        private static AuditModel GetModel(DataDictionaryAudit entity)
        {
            return new AuditModel
            {
                Id = entity.DataDictionaryAuditId,
                Date = entity.DataDictionaryAuditDate,
                Action = (AuditAction)entity.DataDictionaryAuditActionId,
                BeforeJson = entity.DataDictionaryAuditBeforeJson,
                AfterJson = entity.DataDictionaryAuditAfterJson,
                AffectedColumns = entity.DataDictionaryAuditAffectedColumns.Split(',').ToList(),
            };
        }

        private static AuditModel GetModel(UserAccountAudit entity)
        {
            return new AuditModel
            {
                Id = entity.UserAccountAuditId,
                Date = entity.UserAccountAuditDate,
                Action = (AuditAction)entity.UserAccountAuditActionId,
                BeforeJson = entity.UserAccountAuditBeforeJson,
                AfterJson = entity.UserAccountAuditAfterJson,
                AffectedColumns = entity.UserAccountAuditAffectedColumns.Split(',').ToList(),
            };
        }

        private static AuditModel GetModel(ClientAudit entity)
        {
            return new AuditModel
            {
                Id = entity.ClientAuditId,
                Date = entity.ClientAuditDate,
                Action = (AuditAction)entity.ClientAuditActionId,
                BeforeJson = entity.ClientAuditBeforeJson,
                AfterJson = entity.ClientAuditAfterJson,
                AffectedColumns = entity.ClientAuditAffectedColumns.Split(',').ToList(),
            };
        }

        private static AuditModel GetModel(UserAudit entity)
        {
            return new AuditModel
            {
                Id = entity.UserAuditId,
                Date = entity.UserAuditDate,
                Action = (AuditAction)entity.UserAuditActionId,
                BeforeJson = entity.UserAuditBeforeJson,
                AfterJson = entity.UserAuditAfterJson,
                AffectedColumns = entity.UserAuditAffectedColumns.Split(',').ToList(),
            };
        }

        private static AuditModel GetModel(CaseAudit entity)
        {
            return new AuditModel
            {
                Id = entity.CaseAuditId,
                Date = entity.CaseAuditDate,
                Action = (AuditAction)entity.CaseAuditActionId,
                BeforeJson = entity.CaseAuditBeforeJson,
                AfterJson = entity.CaseAuditAfterJson,
                AffectedColumns = entity.CaseAuditAffectedColumns.Split(',').ToList(),
            };
        }

        private static AuditModel GetModel(CaseCommentAudit entity)
        {
            return new AuditModel
            {
                Id = entity.CaseCommentAuditId,
                Date = entity.CaseCommentAuditDate,
                Action = (AuditAction)entity.CaseCommentAuditActionId,
                BeforeJson = entity.CaseCommentAuditBeforeJson,
                AfterJson = entity.CaseCommentAuditAfterJson,
                AffectedColumns = entity.CaseCommentAuditAffectedColumns.Split(',').ToList(),
            };
        }

        private static AuditModel GetModel(CaseDocumentAudit entity)
        {
            return new AuditModel
            {
                Id = entity.CaseDocumentAuditId,
                Date = entity.CaseDocumentAuditDate,
                Action = (AuditAction)entity.CaseDocumentAuditActionId,
                BeforeJson = entity.CaseDocumentAuditBeforeJson,
                AfterJson = entity.CaseDocumentAuditAfterJson,
                AffectedColumns = entity.CaseDocumentAuditAffectedColumns.Split(',').ToList(),
            };
        }

        private static AuditModel GetModel(CaseNoteAudit entity)
        {
            return new AuditModel
            {
                Id = entity.CaseNoteAuditId,
                Date = entity.CaseNoteAuditDate,
                Action = (AuditAction)entity.CaseNoteAuditActionId,
                BeforeJson = entity.CaseNoteAuditBeforeJson,
                AfterJson = entity.CaseNoteAuditAfterJson,
                AffectedColumns = entity.CaseNoteAuditAffectedColumns.Split(',').ToList(),
            };
        }

        private static AuditModel GetModel(CaseUserAudit entity)
        {
            return new AuditModel
            {
                Id = entity.CaseUserAuditId,
                Date = entity.CaseUserAuditDate,
                Action = (AuditAction)entity.CaseUserAuditActionId,
                BeforeJson = entity.CaseUserAuditBeforeJson,
                AfterJson = entity.CaseUserAuditAfterJson,
                AffectedColumns = entity.CaseUserAuditAffectedColumns.Split(',').ToList(),
            };
        }

        #endregion
    }
}
