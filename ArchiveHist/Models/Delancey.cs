using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Models;

[Table("Delancey")]
public partial class Delancey
{
    [Key]
    [Column("D_id")]
    public int DId { get; set; }

    [Column("File Cabinet_Drawer Number")]
    [StringLength(100)]
    [Unicode(false)]
    public string? FileCabinetDrawerNumber { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Address { get; set; }

    public int? Item { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? Type { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? Format { get; set; }

    [Column("Date of Creation")]
    [StringLength(255)]
    [Unicode(false)]
    public string? DateOfCreation { get; set; }

    [Unicode(false)]
    public string? Title { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Creator { get; set; }

    [Unicode(false)]
    public string? Description { get; set; }

    [Column("Makers Marks")]
    [Unicode(false)]
    public string? MakersMarks { get; set; }

    [Column("C_id")]
    public int? CId { get; set; }

    [ForeignKey("CId")]
    [InverseProperty("Delanceys")]
    public virtual Collection? CIdNavigation { get; set; }
}
