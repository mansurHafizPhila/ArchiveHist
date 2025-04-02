using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Models;

[Table("ReportsPub")]
public partial class ReportsPub
{
    [Key]
    [Column("RP_ID")]
    public int RpId { get; set; }

    [StringLength(255)]
    public string? Title { get; set; }

    [Column("Total in Series/Copies")]
    public double? TotalInSeriesCopies { get; set; }

    [Column("Agency/Author(s)")]
    [StringLength(255)]
    public string? AgencyAuthorS { get; set; }

    public DateOnly? Date { get; set; }

    [StringLength(255)]
    public string? Tags { get; set; }

    [StringLength(255)]
    public string? Location { get; set; }

    [StringLength(255)]
    public string? Notes { get; set; }

    [Column("C_id")]
    public int? CId { get; set; }

    [ForeignKey("CId")]
    [InverseProperty("ReportsPubs")]
    public virtual Collection? CIdNavigation { get; set; }
}
