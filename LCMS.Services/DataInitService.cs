using LCMS.Models;
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
            SeedContractData(dbContext);
            SeedContractCommentData(dbContext);
            SeedContractDocumentData(dbContext);
            SeedContractNoteData(dbContext);
            SeedContractUserData(dbContext);
            SeedVendorData(dbContext);
            SeedVendorCommentData(dbContext);
            SeedVendorDocumentData(dbContext);
            SeedVendorNoteData(dbContext);
            SeedVendorUserData(dbContext);

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
            SeedContractAuditData(dbContext, auditService);
            SeedContractCommentAuditData(dbContext, auditService);
            SeedContractDocumentAuditData(dbContext, auditService);
            SeedContractNoteAuditData(dbContext, auditService);
            SeedContractUserAuditData(dbContext, auditService);
            SeedVendorAuditData(dbContext, auditService);
            SeedVendorCommentAuditData(dbContext, auditService);
            SeedVendorDocumentAuditData(dbContext, auditService);
            SeedVendorNoteAuditData(dbContext, auditService);
            SeedVendorUserAuditData(dbContext, auditService);
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
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.AuditAction), Enums.DataDictionaryGroup.AuditAction));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.UserType), Enums.DataDictionaryGroup.UserType));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.ClientType), Enums.DataDictionaryGroup.ClientType));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.CaseType), Enums.DataDictionaryGroup.CaseType));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.CaseStatus), Enums.DataDictionaryGroup.CaseStatus));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.ContractType), Enums.DataDictionaryGroup.ContractType));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.ContractStatus), Enums.DataDictionaryGroup.ContractStatus));
            dataDictionaries.AddRange(GetDataDictionaries(typeof(Enums.DocumentType), Enums.DataDictionaryGroup.DocumentType));

            dbContext.DataDictionaries.AddRange(dataDictionaries);
            dbContext.SaveChanges();
        }

        private static List<DataDictionary> GetDataDictionaries(Type type, Enums.DataDictionaryGroup dataDictionaryGroup)
        {
            var entities = new List<DataDictionary>();
            foreach (var value in Enum.GetValues(type))
            {
                entities.Add(new DataDictionary
                {
                    DataDictionaryIsActive = true,
                    DataDictionaryGroupId = (int)dataDictionaryGroup,
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

        #endregion

        #region Case

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
                                case DocumentType.Excel:
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

        #region Contract

        private static void SeedContractData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.Contracts == null)
            {
                throw new NullReferenceException("Clients");
            }

            // Check for existing data
            if (dbContext.Contracts.Any())
            {
                return;
            }

            var contracts = new List<Contract>();
            foreach (Enums.ContractType contractType in Enum.GetValues(typeof(Enums.ContractType)))
            {
                foreach (Enums.ContractStatus contractStatus in Enum.GetValues(typeof(Enums.ContractStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        contracts.Add(new Contract
                        {
                            ContractIsActive = true,
                            ContractClientId = i,
                            ContractTypeId = (int)contractType,
                            ContractStatusId = (int)contractStatus,
                            ContractTitle = string.Format("Test Contract - {0} - {1} - {2}", contractType, contractStatus, i.ToString("##")),
                        });
                    }
                }
            }

            dbContext.Contracts.AddRange(contracts);
            dbContext.SaveChanges();
        }

        private static void SeedContractCommentData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.ContractComments == null)
            {
                throw new NullReferenceException("ContractComments");
            }

            // Check for existing data
            if (dbContext.ContractComments.Any())
            {
                return;
            }

            var comment = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
            var contractComments = new List<ContractComment>();
            foreach (Enums.ContractType contractType in Enum.GetValues(typeof(Enums.ContractType)))
            {
                foreach (Enums.ContractStatus contractStatus in Enum.GetValues(typeof(Enums.ContractStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            contractComments.Add(new ContractComment
                            {
                                ContractCommentContractId = i,
                                ContractCommentBody = comment
                            });
                        }
                    }
                }
            }

            dbContext.ContractComments.AddRange(contractComments);
            dbContext.SaveChanges();
        }

        private static void SeedContractDocumentData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.ContractDocuments == null)
            {
                throw new NullReferenceException("ContractDocuments");
            }

            // Check for existing data
            if (dbContext.ContractDocuments.Any())
            {
                return;
            }

            var title = "Lorem ipsum dolor sit amet";
            var contractDocuments = new List<ContractDocument>();
            foreach (Enums.ContractType contractType in Enum.GetValues(typeof(Enums.ContractType)))
            {
                foreach (Enums.ContractStatus contractStatus in Enum.GetValues(typeof(Enums.ContractStatus)))
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
                                case DocumentType.Excel:
                                    extension = "xls";
                                    break;
                                case DocumentType.Image:
                                    extension = "jpg";
                                    break;
                            }

                            contractDocuments.Add(new ContractDocument
                            {
                                ContractDocumentContractId = i,
                                ContractDocumentTypeId = (int)documentType,
                                ContractDocumentTitle = string.Format("Test Document - {0} - {1}", documentType, i.ToString("##")),
                                ContractDocumentSummary = title,
                                ContractDocumentOriginalFileName = string.Format("Test-Document-{0}-{1}.{2}", documentType, i.ToString("##"), extension)
                            });
                        }
                    }
                }
            }

            dbContext.ContractDocuments.AddRange(contractDocuments);
            dbContext.SaveChanges();
        }

        private static void SeedContractNoteData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.ContractNotes == null)
            {
                throw new NullReferenceException("ContractNotes");
            }

            // Check for existing data
            if (dbContext.ContractNotes.Any())
            {
                return;
            }

            var note = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
            var contractNotes = new List<ContractNote>();
            foreach (Enums.ContractType contractType in Enum.GetValues(typeof(Enums.ContractType)))
            {
                foreach (Enums.ContractStatus contractStatus in Enum.GetValues(typeof(Enums.ContractStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            contractNotes.Add(new ContractNote
                            {
                                ContractNoteContractId = i,
                                ContractNoteBody = note
                            });
                        }
                    }
                }
            }

            dbContext.ContractNotes.AddRange(contractNotes);
            dbContext.SaveChanges();
        }

        private static void SeedContractUserData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.ContractUsers == null)
            {
                throw new NullReferenceException("ContractUsers");
            }

            // Check for existing data
            if (dbContext.ContractUsers.Any())
            {
                return;
            }

            var contractUsers = new List<ContractUser>();
            foreach (Enums.ContractType contractType in Enum.GetValues(typeof(Enums.ContractType)))
            {
                foreach (Enums.ContractStatus contractStatus in Enum.GetValues(typeof(Enums.ContractStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            // Check to make sure combo doesn't already exist (to prevent duplicate entries)
                            if (!contractUsers.Any(x => x.ContractUserContractId == i && x.ContractUserUserId == j))
                            {
                                contractUsers.Add(new ContractUser
                                {
                                    ContractUserContractId = i,
                                    ContractUserUserId = j
                                });
                            }
                        }
                    }
                }
            }

            dbContext.ContractUsers.AddRange(contractUsers);
            dbContext.SaveChanges();
        }

        #endregion

        #region Vendor

        private static void SeedVendorData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.Vendors == null)
            {
                throw new NullReferenceException("Vendors");
            }

            // Check for existing data
            if (dbContext.Vendors.Any())
            {
                return;
            }

            var Vendors = new List<Vendor>();
            foreach (Enums.VendorType VendorType in Enum.GetValues(typeof(Enums.VendorType)))
            {
                foreach (Enums.VendorStatus VendorStatus in Enum.GetValues(typeof(Enums.VendorStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        Vendors.Add(new Vendor
                        {
                            VendorIsActive = true,
                            VendorTypeId = (int)VendorType,
                            VendorStatusId = (int)VendorStatus,
                            VendorName = string.Format("Test Vendor - {0} - {1} - {2}", VendorType, VendorStatus, i.ToString("##")),
                        });
                    }
                }
            }

            dbContext.Vendors.AddRange(Vendors);
            dbContext.SaveChanges();
        }

        private static void SeedVendorCommentData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.VendorComments == null)
            {
                throw new NullReferenceException("VendorComments");
            }

            // Check for existing data
            if (dbContext.VendorComments.Any())
            {
                return;
            }

            var comment = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
            var vendorComments = new List<VendorComment>();
            foreach (Enums.VendorType vendorType in Enum.GetValues(typeof(Enums.VendorType)))
            {
                foreach (Enums.VendorStatus vendorStatus in Enum.GetValues(typeof(Enums.VendorStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            vendorComments.Add(new VendorComment
                            {
                                VendorCommentVendorId = i,
                                VendorCommentBody = comment
                            });
                        }
                    }
                }
            }

            dbContext.VendorComments.AddRange(vendorComments);
            dbContext.SaveChanges();
        }

        private static void SeedVendorDocumentData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.VendorDocuments == null)
            {
                throw new NullReferenceException("VendorDocuments");
            }

            // Check for existing data
            if (dbContext.VendorDocuments.Any())
            {
                return;
            }

            var title = "Lorem ipsum dolor sit amet";
            var vendorDocuments = new List<VendorDocument>();
            foreach (Enums.VendorType vendorType in Enum.GetValues(typeof(Enums.VendorType)))
            {
                foreach (Enums.VendorStatus vendorStatus in Enum.GetValues(typeof(Enums.VendorStatus)))
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
                                case DocumentType.Excel:
                                    extension = "xls";
                                    break;
                                case DocumentType.Image:
                                    extension = "jpg";
                                    break;
                            }

                            vendorDocuments.Add(new VendorDocument
                            {
                                VendorDocumentVendorId = i,
                                VendorDocumentTypeId = (int)documentType,
                                VendorDocumentTitle = string.Format("Test Document - {0} - {1}", documentType, i.ToString("##")),
                                VendorDocumentSummary = title,
                                VendorDocumentOriginalFileName = string.Format("Test-Document-{0}-{1}.{2}", documentType, i.ToString("##"), extension)
                            });
                        }
                    }
                }
            }

            dbContext.VendorDocuments.AddRange(vendorDocuments);
            dbContext.SaveChanges();
        }

        private static void SeedVendorNoteData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.VendorNotes == null)
            {
                throw new NullReferenceException("VendorNotes");
            }

            // Check for existing data
            if (dbContext.VendorNotes.Any())
            {
                return;
            }

            var note = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
            var vendorNotes = new List<VendorNote>();
            foreach (Enums.VendorType vendorType in Enum.GetValues(typeof(Enums.VendorType)))
            {
                foreach (Enums.VendorStatus vendorStatus in Enum.GetValues(typeof(Enums.VendorStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            vendorNotes.Add(new VendorNote
                            {
                                VendorNoteVendorId = i,
                                VendorNoteBody = note
                            });
                        }
                    }
                }
            }

            dbContext.VendorNotes.AddRange(vendorNotes);
            dbContext.SaveChanges();
        }

        private static void SeedVendorUserData(LCMSDatabaseContext dbContext)
        {
            if (dbContext.VendorUsers == null)
            {
                throw new NullReferenceException("VendorUsers");
            }

            // Check for existing data
            if (dbContext.VendorUsers.Any())
            {
                return;
            }

            var vendorUsers = new List<VendorUser>();
            foreach (Enums.VendorType vendorType in Enum.GetValues(typeof(Enums.VendorType)))
            {
                foreach (Enums.VendorStatus vendorStatus in Enum.GetValues(typeof(Enums.VendorStatus)))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            // Check to make sure combo doesn't already exist (to prevent duplicate entries)
                            if (!vendorUsers.Any(x => x.VendorUserVendorId == i && x.VendorUserUserId == j))
                            {
                                vendorUsers.Add(new VendorUser
                                {
                                    VendorUserVendorId = i,
                                    VendorUserUserId = j
                                });
                            }
                        }
                    }
                }
            }

            dbContext.VendorUsers.AddRange(vendorUsers);
            dbContext.SaveChanges();
        }

        #endregion

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

        #region CaseAudit 

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

        #region ContractAudit 

        private static void SeedContractAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.ContractAudits == null)
            {
                throw new NullReferenceException("ContractAudits");
            }

            // Check for existing data
            if (dbContext.ContractAudits.Any())
            {
                return;
            }

            var entities = dbContext.Contracts.ToList();
            foreach (var entity in entities)
            {
                service.CreateContract(entity, 1);
            }
        }

        private static void SeedContractCommentAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.ContractCommentAudits == null)
            {
                throw new NullReferenceException("ContractCommentAudits");
            }

            // Check for existing data
            if (dbContext.ContractCommentAudits.Any())
            {
                return;
            }

            var entities = dbContext.ContractComments.ToList();
            foreach (var entity in entities)
            {
                service.CreateContractComment(entity, 1);
            }
        }

        private static void SeedContractDocumentAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.ContractDocumentAudits == null)
            {
                throw new NullReferenceException("ContractDocumentAudits");
            }

            // Check for existing data
            if (dbContext.ContractDocumentAudits.Any())
            {
                return;
            }

            var entities = dbContext.ContractDocuments.ToList();
            foreach (var entity in entities)
            {
                service.CreateContractDocument(entity, 1);
            }
        }

        private static void SeedContractNoteAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.ContractNoteAudits == null)
            {
                throw new NullReferenceException("ContractNoteAudits");
            }

            // Check for existing data
            if (dbContext.ContractNoteAudits.Any())
            {
                return;
            }

            var entities = dbContext.ContractNotes.ToList();
            foreach (var entity in entities)
            {
                service.CreateContractNote(entity, 1);
            }
        }

        private static void SeedContractUserAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.ContractUserAudits == null)
            {
                throw new NullReferenceException("ContractUserAudits");
            }

            // Check for existing data
            if (dbContext.ContractUserAudits.Any())
            {
                return;
            }

            var entities = dbContext.ContractUsers.ToList();
            foreach (var entity in entities)
            {
                service.CreateContractUser(entity, 1);
            }
        }

        #endregion

        #region VendorAudit 

        private static void SeedVendorAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.VendorAudits == null)
            {
                throw new NullReferenceException("VendorAudits");
            }

            // Check for existing data
            if (dbContext.VendorAudits.Any())
            {
                return;
            }

            var entities = dbContext.Vendors.ToList();
            foreach (var entity in entities)
            {
                service.CreateVendor(entity, 1);
            }
        }

        private static void SeedVendorCommentAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.VendorCommentAudits == null)
            {
                throw new NullReferenceException("VendorCommentAudits");
            }

            // Check for existing data
            if (dbContext.VendorCommentAudits.Any())
            {
                return;
            }

            var entities = dbContext.VendorComments.ToList();
            foreach (var entity in entities)
            {
                service.CreateVendorComment(entity, 1);
            }
        }

        private static void SeedVendorDocumentAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.VendorDocumentAudits == null)
            {
                throw new NullReferenceException("VendorDocumentAudits");
            }

            // Check for existing data
            if (dbContext.VendorDocumentAudits.Any())
            {
                return;
            }

            var entities = dbContext.VendorDocuments.ToList();
            foreach (var entity in entities)
            {
                service.CreateVendorDocument(entity, 1);
            }
        }

        private static void SeedVendorNoteAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.VendorNoteAudits == null)
            {
                throw new NullReferenceException("VendorNoteAudits");
            }

            // Check for existing data
            if (dbContext.VendorNoteAudits.Any())
            {
                return;
            }

            var entities = dbContext.VendorNotes.ToList();
            foreach (var entity in entities)
            {
                service.CreateVendorNote(entity, 1);
            }
        }

        private static void SeedVendorUserAuditData(LCMSDatabaseContext dbContext, IAuditService service)
        {
            if (dbContext.VendorUserAudits == null)
            {
                throw new NullReferenceException("VendorUserAudits");
            }

            // Check for existing data
            if (dbContext.VendorUserAudits.Any())
            {
                return;
            }

            var entities = dbContext.VendorUsers.ToList();
            foreach (var entity in entities)
            {
                service.CreateVendorUser(entity, 1);
            }
        }

        #endregion

        #endregion
    }
}
