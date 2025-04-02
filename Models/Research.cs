using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Models
{
    [Table("Research")]
    public partial class Research
    {
        [Key]
        [Column("R_id")]
        public int RId { get; set; }
        [Column("Researcher Name")]
        [StringLength(100)]
        [Unicode(false)]
        public string? ResearcherName { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string? Team { get; set; }
        [Column("Visited archives")]
        [StringLength(10)]
        [Unicode(false)]
        public string? VisitedArchives { get; set; }
        [Column("Topic Category")]
        [StringLength(30)]
        [Unicode(false)]
        public string? TopicCategory { get; set; }
        [Column("Specific Inquiry")]
        [Unicode(false)]
        public string? SpecificInquiry { get; set; }
        [Column("Description of Tasks Involved")]
        [Unicode(false)]
        public string? DescriptionOfTasksInvolved { get; set; }
        [Column("Scans Taken")]
        [StringLength(10)]
        [Unicode(false)]
        public string? ScansTaken { get; set; }
        [Column("Receiving Archivist")]
        [StringLength(50)]
        [Unicode(false)]
        public string? ReceivingArchivist { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Notes { get; set; }
        [Column("C_id")]
        public int? CId { get; set; }

        [ForeignKey("CId")]
        [InverseProperty("Researches")]
        public virtual Collection? CIdNavigation { get; set; }
    }
}
