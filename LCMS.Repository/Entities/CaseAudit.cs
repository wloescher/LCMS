﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LCMS.Repository.Entities;

[Table("CaseAudit")]
public partial class CaseAudit
{
    [Key]
    public int CaseAuditId { get; set; }

    public int CaseAuditActionId { get; set; }

    public int CaseAuditCaseId { get; set; }

    public int CaseAuditUserId { get; set; }

    public DateTime CaseAuditDate { get; set; }

    [Required]
    public string CaseAuditBeforeJson { get; set; }

    [Required]
    public string CaseAuditAfterJson { get; set; }

    [Required]
    public string CaseAuditAffectedColumns { get; set; }

    [ForeignKey("CaseAuditActionId")]
    [InverseProperty("CaseAudits")]
    public virtual DataDictionary CaseAuditAction { get; set; }

    [ForeignKey("CaseAuditCaseId")]
    [InverseProperty("CaseAudits")]
    public virtual Case CaseAuditCase { get; set; }

    [ForeignKey("CaseAuditUserId")]
    [InverseProperty("CaseAudits")]
    public virtual User CaseAuditUser { get; set; }
}