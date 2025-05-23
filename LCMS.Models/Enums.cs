﻿using System.ComponentModel.DataAnnotations;

namespace LCMS.Models
{
    public class Enums
    {
        public enum DataDictionaryGroup
        {
            [Display(Name = "Audit Action")]
            AuditAction = 1,

            [Display(Name = "User Type")]
            UserType = 2,

            [Display(Name = "Client Type")]
            ClientType = 3,

            [Display(Name = "Case Type")]
            CaseType = 4,

            [Display(Name = "Case Status")]
            CaseStatus = 5,

            [Display(Name = "Document Type")]
            DocumentType = 6,
        }

        public enum AuditAction
        {
            Create = 1,
            Update = 2,
            Delete = 3,
            Read = 4,
            Login = 5,
            Logout = 6,
            Error = 7,
            LoginFailed = 8,
            PasswordChanged = 9,
        }

        public enum UserType
        {
            Admin = 1,
            Attorney = 2,
            Paralegal = 3,
        }

        public enum ClientType
        {
            Internal = 1,
            External = 2,
            Lead = 3,
        }

        public enum CaseType
        {
            Criminal = 1,
            Civil = 2,
            [Display(Name = "Family Law")]
            FamilyLaw = 3,
        }

        public enum CaseStatus
        {
            Open = 1,
            [Display(Name = "In Progress")]
            InProgress = 2,
            Closed = 3,
        }

        public enum DocumentType
        {
            Text = 1,
            Word = 2,
            PDF = 3,
            Spreadsheet = 4,
            Image = 5,
        }
    }
}
