﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LCMS.Repository.Entities;

[Keyless]
public partial class CaseCommentView
{
    public int CaseCommentId { get; set; }

    public Guid CaseCommentGuid { get; set; }

    public int CaseId { get; set; }

    [StringLength(150)]
    public string CaseTitle { get; set; }

    [StringLength(500)]
    public string Comment { get; set; }
}