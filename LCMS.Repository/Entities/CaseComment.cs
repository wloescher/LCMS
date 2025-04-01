﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LCMS.Repository.Entities;

[Table("CaseComment")]
public partial class CaseComment
{
    [Key]
    public int CaseCommentId { get; set; }

    public Guid CaseCommentGuid { get; set; }

    public bool CaseCommentIsDeleted { get; set; }

    public int CaseCommentCaseId { get; set; }

    [StringLength(500)]
    public string CaseCommentBody { get; set; }

    [InverseProperty("CaseCommentAuditCaseComment")]
    public virtual ICollection<CaseCommentAudit> CaseCommentAudits { get; set; } = new List<CaseCommentAudit>();

    [ForeignKey("CaseCommentCaseId")]
    [InverseProperty("CaseComments")]
    public virtual Case CaseCommentCase { get; set; }
}