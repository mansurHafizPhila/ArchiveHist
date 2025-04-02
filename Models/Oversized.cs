using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Models
{
    [Table("Oversized")]
    public partial class Oversized
    {
        [Key]
        [Column("O_id")]
        public int OId { get; set; }
        [Column("Building Name")]
        [Unicode(false)]
        public string? BuildingName { get; set; }
        [Column("Year/Range")]
        [StringLength(50)]
        [Unicode(false)]
        public string? YearRange { get; set; }
        [Column("Company/Architect")]
        [Unicode(false)]
        public string? CompanyArchitect { get; set; }
        [Unicode(false)]
        public string? Drawer { get; set; }
        [Column("Side Notes")]
        [Unicode(false)]
        public string? SideNotes { get; set; }
        [Column("C_id")]
        public int? CId { get; set; }

        [ForeignKey("CId")]
        [InverseProperty("Oversizeds")]
        public virtual Collection? CIdNavigation { get; set; }
    }
}
