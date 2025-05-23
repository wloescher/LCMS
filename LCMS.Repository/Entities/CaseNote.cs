﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LCMS.Repository.Entities;

[Table("CaseNote")]
public partial class CaseNote
{
    [Key]
    public int CaseNoteId { get; set; }

    public Guid CaseNoteGuid { get; set; }

    public bool CaseNoteIsDeleted { get; set; }

    public int CaseNoteCaseId { get; set; }

    [StringLength(500)]
    public string CaseNoteBody { get; set; }

    [InverseProperty("CaseNoteAuditCaseNote")]
    public virtual ICollection<CaseNoteAudit> CaseNoteAudits { get; set; } = new List<CaseNoteAudit>();

    [ForeignKey("CaseNoteCaseId")]
    [InverseProperty("CaseNotes")]
    public virtual Case CaseNoteCase { get; set; }
}