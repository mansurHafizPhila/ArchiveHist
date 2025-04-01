using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Model
{
    [Table("Trunk")]
    public partial class Trunk
    {
        [Key]
        [Column("T_id")]
        public int TId { get; set; }
        [Column("Building Name/Plan Title")]
        [Unicode(false)]
        public string? BuildingNamePlanTitle { get; set; }
        [Column("Plan Year", TypeName = "date")]
        public DateTime? PlanYear { get; set; }
        [Column("Architect/ Firm/ Associated")]
        [Unicode(false)]
        public string? ArchitectFirmAssociated { get; set; }
        [Column("Folder Name")]
        [StringLength(50)]
        [Unicode(false)]
        public string? FolderName { get; set; }
        [Unicode(false)]
        public string? Notes { get; set; }
        [Unicode(false)]
        public string? Links { get; set; }
        [Column("C_id")]
        public int? CId { get; set; }

        [ForeignKey("CId")]
        [InverseProperty("Trunks")]
        public virtual Collection? CIdNavigation { get; set; }
    }
}
