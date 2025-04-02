using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Models;

[Table("Collection")]
public partial class Collection
{
    [Key]
    [Column("C_id")]
    public int CId { get; set; }

    [Column("C_name")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CName { get; set; }

    [Column("description")]
    [Unicode(false)]
    public string? Description { get; set; }

    [InverseProperty("CIdNavigation")]
    public virtual ICollection<AudioFile> AudioFiles { get; set; } = new List<AudioFile>();

    [InverseProperty("CIdNavigation")]
    public virtual ICollection<Delancey> Delanceys { get; set; } = new List<Delancey>();

    [InverseProperty("CIdNavigation")]
    public virtual ICollection<Map> Maps { get; set; } = new List<Map>();

    [InverseProperty("CIdNavigation")]
    public virtual ICollection<Oversized> Oversizeds { get; set; } = new List<Oversized>();

    [InverseProperty("CIdNavigation")]
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();

    [InverseProperty("CIdNavigation")]
    public virtual ICollection<PoisonBook> PoisonBooks { get; set; } = new List<PoisonBook>();

    [InverseProperty("CIdNavigation")]
    public virtual ICollection<ReportsPub> ReportsPubs { get; set; } = new List<ReportsPub>();

    [InverseProperty("CIdNavigation")]
    public virtual ICollection<Research> Researches { get; set; } = new List<Research>();

    [InverseProperty("CIdNavigation")]
    public virtual ICollection<Trunk> Trunks { get; set; } = new List<Trunk>();
}
