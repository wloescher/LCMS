﻿using LCMS.Models;
using LCMS.Repository.Entities;
using LCMS.Services.Interfaces;
using LCMS.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static LCMS.Models.Enums;
using DataDictionary = LCMS.Repository.Entities.DataDictionary;
using DataDictionaryGroup = LCMS.Repository.Entities.DataDictionaryGroup;

namespace LCMS.Services
{
    public static class DataInitService
    {
        #region Public Methods

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var dbContext = new LCMSDatabaseContext(serviceProvider.GetRequiredService<DbContextOptions<LCMSDatabaseContext>>());
            if (dbContext == null)
            {
                throw new NullReferenceException("LCMSDatabaseContext");
            }

            // Seed data
            SeedDataDictionaryGroupData(dbContext);
            SeedDataDictionaryData(dbContext);
            SeedUserAccountData(dbContext);
            SeedUserData(dbContext);
            SeedClientData(dbContext);
            SeedCaseData(dbContext);
            SeedCaseCommentData(dbContext);
            SeedCaseDocumentData(dbContext);
            SeedCaseNoteData(dbContext);
            SeedCaseUserData(dbContext);

            // Seed audit data
            using var scope = serviceProvider.CreateScope();
            var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
            SeedDataDictionaryGroupAuditData(dbContext, auditService);
            SeedDataDictionaryAuditData(dbContext, auditService);
            SeedUserAccountAuditData(dbContext, auditService);
            SeedUserAuditData(dbContext, auditService);
            SeedClientAuditData(dbContext, auditService);
            SeedCaseAuditData(dbContext, auditService);
            SeedCaseCommentAuditData(dbContext, auditService);
            SeedCaseDocumentAuditData(dbContext, auditService);
            SeedCaseNoteAuditData(dbContext, auditService);
            SeedCaseUserAuditData(dbContext, auditService);

        }

        #endregion

        #region Private Methods

        #region Seed Data

        private static void SeedDataDictionaryGroupData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.DataDictionaryGroups == null)
            {
                throw new NullReferenceException("DataDictionaryGroups");
            }

            // Check for existing data
            if (dbContext.DataDictionaryGroups.Any())
            {
                return;
            }

            var dataDictionaryGroups = new List<DataDictionaryGroup>();
            foreach (Enums.DataDictionaryGroup value in Enum.GetValues(typeof(Enums.DataDictionaryGroup)))
            {
                dataDictionaryGroups.Add(new DataDictionaryGroup
                {
                    DataDictionaryGroupIsActive = true,
                    DataDictionaryGroupName = value.ToString(),
                });
            }

            dbContext.DataDictionaryGroups.AddRange(dataDictionaryGroups);
            dbContext.SaveChanges();
        }

        private static void SeedDataDictionaryData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.DataDictionaries == null)
            {
                throw new NullReferenceException("DataDictionaries");
            }

            // Check for existing data
            if (dbContext.DataDictionaries.Any())
            {
                return;
            }

            var dataDictionaries = new List<DataDictionary>();
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.AuditAction)));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.UserType)));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.ClientType)));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.CaseType)));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.CaseStatus)));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.DocumentType)));

            dbContext.DataDictionaries.AddRange(dataDictionaries);
            dbContext.SaveChanges();
        }

        private static List<DataDictionary> GetDataDictionaries(Type type)
        {
            var entities = new List<DataDictionary>();
            foreach (var value in Enum.GetValues(type))
            {
                entities.Add(new DataDictionary
                {
                    DataDictionaryIsActive = true,
                    DataDictionaryGroupId = (int)Enums.DataDictionaryGroup.UserType,
                    DataDictionaryKey = value.ToString(),
                    DataDictionaryValue = (int)value,
                });
            }
            return entities;
        }

        private static void SeedUserAccountData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.UserAccounts == null)
            {
                throw new NullReferenceException("UserAccounts");
            }

            // Check for existing data
            if (dbContext.UserAccounts.Any())
            {
                return;
            }

            string userName;
            var userAccounts = new List<UserAccount>();
            foreach (Enums.UserType userType in Enum.GetValues(typeof(Enums.UserType)))
            {
                userName = userType.ToString().ToLower();
                userAccounts.Add(new UserAccount
                {
                    UserAccountIsActive = true,
                    UserAccountUserName = userName,
                    UserAccountPassword = userName,
                    UserAccountPasswordHash = SecurityUtility.PasswordHash(userName),
                });
            }

            userName = "inactive-account";
            userAccounts.Add(new UserAccount
            {
                UserAccountUserName = userName,
                UserAccountPassword = userName,
                UserAccountPasswordHash = SecurityUtility.PasswordHash(userName),
            });

            userName = "deleted-account";
            userAccounts.Add(new UserAccount
            {
                UserAccountIsActive = true,
                UserAccountIsDeleted = true,
                UserAccountUserName = userName,
                UserAccountPassword = userName,
                UserAccountPasswordHash = SecurityUtility.PasswordHash(userName),
            });

            userName = "locked-out";
            userAccounts.Add(new UserAccount
            {
                UserAccountIsActive = true,
                UserAccountUserName = userName,
                UserAccountPassword = userName,
                UserAccountPasswordHash = SecurityUtility.PasswordHash(userName),
                UserAccountPasswordAttemptCount = 99,
            });

            dbContext.UserAccounts.AddRange(userAccounts);
            dbContext.SaveChanges();
        }

        private static void SeedUserData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.Users == null)
            {
                throw new NullReferenceException("Users");
            }

            // Check for existing data
            if (dbContext.Users.Any())
            {
                return;
            }

            dbContext.Users.AddRange(
                new User
                {
                    UserIsActive = true,
                    UserUserAccountId = 1,
                    UserTypeId = (int)UserType.Admin,
                    UserFirstName = "Test",
                    UserLastName = "Admin",
                    UserEmailAddress = "admin@lcms.com",
                },
                new User
                {
                    UserIsActive = true,
                    UserUserAccountId = 2,
                    UserTypeId = (int)UserType.Attorney,
                    UserFirstName = "Test",
                    UserLastName = "Attorney",
                    UserEmailAddress = "attorney@lcms.com",
                },
                new User
                {
                    UserIsActive = true,
                    UserUserAccountId = 3,
                    UserTypeId = (int)UserType.Paralegal,
                    UserFirstName = "Test",
                    UserLastName = "Paralegal",
                    UserEmailAddress = "paralegal@lcms.com",
                },
                new User
                {
                    UserIsActive = true,
                    UserUserAccountId = 4,
                    UserTypeId = (int)UserType.Attorney,
                    UserFirstName = "Test",
                    UserLastName = "Inactive Account",
                    UserEmailAddress = "inactive-account@lcms.com",
                },
                new User
                {
                    UserTypeId = (int)UserType.Attorney,
                    UserFirstName = "Test",
                    UserLastName = "Inactive User",
                    UserEmailAddress = "inactive-user@lcms.com",
                });

            dbContext.SaveChanges();
        }

        private static void SeedClientData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.Clients == null)
            {
                throw new NullReferenceException("Clients");
            }

            // Check for existing data
            if (dbContext.Clients.Any())
            {
                return;
            }

            var clients = new List<Client>();
            foreach (Enums.ClientType clientType in Enum.GetValues(typeof(Enums.ClientType)))
            {
                for (int i = 1; i <= 10; i++)
                {
                    clients.Add(new Client
                    {
                        ClientIsActive = true,
                        ClientTypeId = (int)clientType,
                        ClientName = string.Format("Test Client - {0} - {1}", clientType, i.ToString("##")),
                    });
                }
            }

            dbContext.Clients.AddRange(clients);
            dbContext.SaveChanges();
        }

        private static void SeedCaseData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.Cases == null)
            {
                throw new NullReferenceException("Clients");
            }

            // Check for existing data
            if (dbContext.Cases.Any())
            {
                return;
            }

            var cases = new List<Case>();
            foreach (Enums.CaseType caseType in Enum.GetValues(typeof(Enums.CaseType)))
            {
                foreach (Enums.CaseStatus caseStatus in Enum.GetValues(typeof(Enums.CaseStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        cases.Add(new Case
                        {
                            CaseIsActive = true,
                            CaseClientId = i,
                            CaseTypeId = (int)caseType,
                            CaseStatusId = (int)caseStatus,
                            CaseTitle = string.Format("Test Case - {0} - {1} - {2}", caseType, caseStatus, i.ToString("##")),
                        });
                    }
                }
            }

            dbContext.Cases.AddRange(cases);
            dbContext.SaveChanges();
        }

        private static void SeedCaseCommentData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.CaseComments == null)
            {
                throw new NullReferenceException("CaseComments");
            }

            // Check for existing data
            if (dbContext.CaseComments.Any())
            {
                return;
            }

            var comment = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
            var caseComments = new List<CaseComment>();
            foreach (Enums.CaseType caseType in Enum.GetValues(typeof(Enums.CaseType)))
            {
                foreach (Enums.CaseStatus caseStatus in Enum.GetValues(typeof(Enums.CaseStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            caseComments.Add(new CaseComment
                            {
                                CaseCommentCaseId = i,
                                CaseCommentBody = comment
                            });
                        }
                    }
                }
            }

            dbContext.CaseComments.AddRange(caseComments);
            dbContext.SaveChanges();
        }

        private static void SeedCaseDocumentData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.CaseDocuments == null)
            {
                throw new NullReferenceException("CaseDocuments");
            }

            // Check for existing data
            if (dbContext.CaseDocuments.Any())
            {
                return;
            }

            var title = "Lorem ipsum dolor sit amet";
            var caseDocuments = new List<CaseDocument>();
            foreach (Enums.CaseType caseType in Enum.GetValues(typeof(Enums.CaseType)))
            {
                foreach (Enums.CaseStatus caseStatus in Enum.GetValues(typeof(Enums.CaseStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        foreach (Enums.DocumentType documentType in Enum.GetValues(typeof(Enums.DocumentType)))
                        {
                            var extension = string.Empty;
                            switch (documentType)
                            {
                                case DocumentType.Text:
                                    extension = "txt";
                                    break;
                                case DocumentType.Word:
                                    extension = "doc";
                                    break;
                                case DocumentType.PDF:
                                    extension = "pdf";
                                    break;
                                case DocumentType.Spreadsheet:
                                    extension = "xls";
                                    break;
                                case DocumentType.Image:
                                    extension = "jpg";
                                    break;
                            }

                            caseDocuments.Add(new CaseDocument
                            {
                                CaseDocumentCaseId = i,
                                CaseDocumentTypeId = (int)documentType,
                                CaseDocumentTitle = string.Format("Test Document - {0} - {1}", documentType, i.ToString("##")),
                                CaseDocumentSummary = title,
                                CaseDocumentOriginalFileName = string.Format("Test-Document-{0}-{1}.{2}", documentType, i.ToString("##"), extension)
                            });
                        }
                    }
                }
            }

            dbContext.CaseDocuments.AddRange(caseDocuments);
            dbContext.SaveChanges();
        }

        private static void SeedCaseNoteData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.CaseNotes == null)
            {
                throw new NullReferenceException("CaseNotes");
            }

            // Check for existing data
            if (dbContext.CaseNotes.Any())
            {
                return;
            }

            var note = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
            var caseNotes = new List<CaseNote>();
            foreach (Enums.CaseType caseType in Enum.GetValues(typeof(Enums.CaseType)))
            {
                foreach (Enums.CaseStatus caseStatus in Enum.GetValues(typeof(Enums.CaseStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            caseNotes.Add(new CaseNote
                            {
                                CaseNoteCaseId = i,
                                CaseNoteBody = note
                            });
                        }
                    }
                }
            }

            dbContext.CaseNotes.AddRange(caseNotes);
            dbContext.SaveChanges();
        }

        private static void SeedCaseUserData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.CaseUsers == null)
            {
                throw new NullReferenceException("CaseUsers");
            }

            // Check for existing data
            if (dbContext.CaseUsers.Any())
            {
                return;
            }

            var caseUsers = new List<CaseUser>();
            foreach (Enums.CaseType caseType in Enum.GetValues(typeof(Enums.CaseType)))
            {
                foreach (Enums.CaseStatus caseStatus in Enum.GetValues(typeof(Enums.CaseStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            // Check to make sure combo doesn't already exist (to prevent duplicate entries)
                            if (!caseUsers.Any(x => x.CaseUserCaseId == i && x.CaseUserUserId == j))
                            {
                                caseUsers.Add(new CaseUser
                                {
                                    CaseUserCaseId = i,
                                    CaseUserUserId = j
                                });
                            }
                        }
                    }
                }
            }

            dbContext.CaseUsers.AddRange(caseUsers);
            dbContext.SaveChanges();
        }

        #endregion 

        #region Seed Audit Data

        private static void SeedDataDictionaryGroupAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.DataDictionaryGroupAudits == null)
            {
                throw new NullReferenceException("DataDictionaryGroupAudits");
            }

            // Check for existing data
            if (dbContext.DataDictionaryGroupAudits.Any())
            {
                return;
            }

            var entities = dbContext.DataDictionaryGroups.ToList();
            foreach (var entity in entities)
            {
                service.CreateDataDictionaryGroup(entity, 1);
            }
        }

        private static void SeedDataDictionaryAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.DataDictionaryAudits == null)
            {
                throw new NullReferenceException("DataDictionaryAudits");
            }

            // Check for existing data
            if (dbContext.DataDictionaryAudits.Any())
            {
                return;
            }

            var entities = dbContext.DataDictionaries.ToList();
            foreach (var entity in entities)
            {
                service.CreateDataDictionary(entity, 1);
            }
        }

        private static void SeedUserAccountAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.UserAccountAudits == null)
            {
                throw new NullReferenceException("UserAccountAudits");
            }

            // Check for existing data
            if (dbContext.UserAccountAudits.Any())
            {
                return;
            }

            var entities = dbContext.UserAccounts.ToList();
            foreach (var entity in entities)
            {
                service.CreateUserAccount(entity, 1);
            }
        }

        private static void SeedUserAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.UserAudits == null)
            {
                throw new NullReferenceException("UserAudits");
            }

            // Check for existing data
            if (dbContext.UserAudits.Any())
            {
                return;
            }

            var entities = dbContext.Users.ToList();
            foreach (var entity in entities)
            {
                service.CreateUser(entity, 1);
            }
        }

        private static void SeedClientAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.ClientAudits == null)
            {
                throw new NullReferenceException("ClientAudits");
            }

            // Check for existing data
            if (dbContext.ClientAudits.Any())
            {
                return;
            }

            var entities = dbContext.Clients.ToList();
            foreach (var entity in entities)
            {
                service.CreateClient(entity, 1);
            }
        }

        private static void SeedCaseAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.CaseAudits == null)
            {
                throw new NullReferenceException("CaseAudits");
            }

            // Check for existing data
            if (dbContext.CaseAudits.Any())
            {
                return;
            }

            var entities = dbContext.Cases.ToList();
            foreach (var entity in entities)
            {
                service.CreateCase(entity, 1);
            }
        }

        private static void SeedCaseCommentAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.CaseCommentAudits == null)
            {
                throw new NullReferenceException("CaseCommentAudits");
            }

            // Check for existing data
            if (dbContext.CaseCommentAudits.Any())
            {
                return;
            }

            var entities = dbContext.CaseComments.ToList();
            foreach (var entity in entities)
            {
                service.CreateCaseComment(entity, 1);
            }
        }

        private static void SeedCaseDocumentAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.CaseDocumentAudits == null)
            {
                throw new NullReferenceException("CaseDocumentAudits");
            }

            // Check for existing data
            if (dbContext.CaseDocumentAudits.Any())
            {
                return;
            }

            var entities = dbContext.CaseDocuments.ToList();
            foreach (var entity in entities)
            {
                service.CreateCaseDocument(entity, 1);
            }
        }

        private static void SeedCaseNoteAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.CaseNoteAudits == null)
            {
                throw new NullReferenceException("CaseNoteAudits");
            }

            // Check for existing data
            if (dbContext.CaseNoteAudits.Any())
            {
                return;
            }

            var entities = dbContext.CaseNotes.ToList();
            foreach (var entity in entities)
            {
                service.CreateCaseNote(entity, 1);
            }
        }

        private static void SeedCaseUserAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.CaseUserAudits == null)
            {
                throw new NullReferenceException("CaseUserAudits");
            }

            // Check for existing data
            if (dbContext.CaseUserAudits.Any())
            {
                return;
            }

            var entities = dbContext.CaseUsers.ToList();
            foreach (var entity in entities)
            {
                service.CreateCaseUser(entity, 1);
            }
        }

        #endregion

        #endregion
    }
}
