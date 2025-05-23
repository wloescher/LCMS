﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LCMS.Repository.Entities;

[Table("UserAudit")]
public partial class UserAudit
{
    [Key]
    public int UserAuditId { get; set; }

    public int UserAuditActionId { get; set; }

    public int UserAuditUserId { get; set; }

    [Column("UserAuditUserId_Source")]
    public int UserAuditUserIdSource { get; set; }

    public DateTime UserAuditDate { get; set; }

    [Required]
    public string UserAuditBeforeJson { get; set; }

    [Required]
    public string UserAuditAfterJson { get; set; }

    [Required]
    public string UserAuditAffectedColumns { get; set; }

    [ForeignKey("UserAuditActionId")]
    [InverseProperty("UserAudits")]
    public virtual DataDictionary UserAuditAction { get; set; }

    [ForeignKey("UserAuditUserId")]
    [InverseProperty("UserAuditUserAuditUsers")]
    public virtual User UserAuditUser { get; set; }

    [ForeignKey("UserAuditUserIdSource")]
    [InverseProperty("UserAuditUserAuditUserIdSourceNavigations")]
    public virtual User UserAuditUserIdSourceNavigation { get; set; }
}