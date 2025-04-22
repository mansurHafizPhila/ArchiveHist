using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Models;

public partial class AudioFile
{
    [Key]
    [Column("A_id")]
    public int AId { get; set; }

    [Column("Link Name")]
    [Unicode(false)]
    public string? LinkName { get; set; }

    [Column("C_id")]
    public int? CId { get; set; }

    [ForeignKey("CId")]
    [InverseProperty("AudioFiles")]
    public virtual Collection? CIdNavigation { get; set; }

    [NotMapped]
    public string DisplayName { get; set; } = string.Empty;
}
