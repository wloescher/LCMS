﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LCMS.Repository.Entities;

[Keyless]
public partial class CaseDocumentView
{
    public int CaseDocumentId { get; set; }

    public Guid CaseDocumentGuid { get; set; }

    public int CaseId { get; set; }

    [StringLength(150)]
    public string CaseTitle { get; set; }

    public int CaseDocumentTypeId { get; set; }

    [Required]
    [StringLength(50)]
    public string DocumentType { get; set; }

    [StringLength(255)]
    public string DocumentTitle { get; set; }

    [StringLength(500)]
    public string DocumentSummary { get; set; }

    [StringLength(255)]
    public string OriginalFileName { get; set; }
}