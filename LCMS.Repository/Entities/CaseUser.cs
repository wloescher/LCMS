﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LCMS.Repository.Entities;

[Table("CaseUser")]
[Index("CaseUserCaseId", "CaseUserUserId", Name = "UQ_CaseUser", IsUnique = true)]
public partial class CaseUser
{
    [Key]
    public int CaseUserId { get; set; }

    public int CaseUserCaseId { get; set; }

    public int CaseUserUserId { get; set; }

    public bool CaseUserIsDeleted { get; set; }

    [InverseProperty("CaseUserAuditCaseUser")]
    public virtual ICollection<CaseUserAudit> CaseUserAudits { get; set; } = new List<CaseUserAudit>();

    [ForeignKey("CaseUserCaseId")]
    [InverseProperty("CaseUsers")]
    public virtual Case CaseUserCase { get; set; }

    [ForeignKey("CaseUserUserId")]
    [InverseProperty("CaseUsers")]
    public virtual User CaseUserUser { get; set; }
}