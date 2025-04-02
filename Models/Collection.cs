using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Models
{
    [Table("Collection")]
    public partial class Collection
    {
        public Collection()
        {
            AudioFiles = new HashSet<AudioFile>();
            Delanceys = new HashSet<Delancey>();
            Maps = new HashSet<Map>();
            Oversizeds = new HashSet<Oversized>();
            Photos = new HashSet<Photo>();
            PoisonBooks = new HashSet<PoisonBook>();
            ReportsPubs = new HashSet<ReportsPub>();
            Researches = new HashSet<Research>();
            Trunks = new HashSet<Trunk>();
        }

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
        public virtual ICollection<AudioFile> AudioFiles { get; set; }
        [InverseProperty("CIdNavigation")]
        public virtual ICollection<Delancey> Delanceys { get; set; }
        [InverseProperty("CIdNavigation")]
        public virtual ICollection<Map> Maps { get; set; }
        [InverseProperty("CIdNavigation")]
        public virtual ICollection<Oversized> Oversizeds { get; set; }
        [InverseProperty("CIdNavigation")]
        public virtual ICollection<Photo> Photos { get; set; }
        [InverseProperty("CIdNavigation")]
        public virtual ICollection<PoisonBook> PoisonBooks { get; set; }
        [InverseProperty("CIdNavigation")]
        public virtual ICollection<ReportsPub> ReportsPubs { get; set; }
        [InverseProperty("CIdNavigation")]
        public virtual ICollection<Research> Researches { get; set; }
        [InverseProperty("CIdNavigation")]
        public virtual ICollection<Trunk> Trunks { get; set; }
    }
}
