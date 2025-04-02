using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Models
{
    [Table("Photo")]
    public partial class Photo
    {
        [Key]
        [Column("P_id")]
        public int PId { get; set; }
        [Unicode(false)]
        public string? Title { get; set; }
        [Column("Artist/Agency")]
        [StringLength(50)]
        [Unicode(false)]
        public string? ArtistAgency { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Year { get; set; }
        [Unicode(false)]
        public string? Link { get; set; }
        [Unicode(false)]
        public string? Notes { get; set; }
        [Column("C_id")]
        public int? CId { get; set; }

        [ForeignKey("CId")]
        [InverseProperty("Photos")]
        public virtual Collection? CIdNavigation { get; set; }
    }
}
