using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Model
{
    public partial class PoisonBook
    {
        [Key]
        [Column("P_ID")]
        public int PId { get; set; }
        [StringLength(255)]
        public string? Title { get; set; }
        [StringLength(255)]
        public string? Location { get; set; }
        [StringLength(255)]
        public string? Author { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }
        public bool Poison { get; set; }
        [Column("ARSENIC WARNING")]
        public bool ArsenicWarning { get; set; }
        [Column("C_id")]
        public int? CId { get; set; }

        [ForeignKey("CId")]
        [InverseProperty("PoisonBooks")]
        public virtual Collection? CIdNavigation { get; set; }
    }
}
