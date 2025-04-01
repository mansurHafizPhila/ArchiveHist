using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiveHist.Model
{
    public partial class Map
    {
        [Key]
        [Column("M_id")]
        public int MId { get; set; }
        [Column("Map Name")]
        [Unicode(false)]
        public string? MapName { get; set; }
        [Column("Year/Range")]
        [StringLength(50)]
        [Unicode(false)]
        public string? YearRange { get; set; }
        [Column("Artist/Manufacturer")]
        [StringLength(50)]
        [Unicode(false)]
        public string? ArtistManufacturer { get; set; }
        [Column("Digitized Link")]
        [Unicode(false)]
        public string? DigitizedLink { get; set; }
        [StringLength(30)]
        [Unicode(false)]
        public string? Removed { get; set; }
        [Column("C_id")]
        public int? CId { get; set; }

        [ForeignKey("CId")]
        [InverseProperty("Maps")]
        public virtual Collection? CIdNavigation { get; set; }
    }
}
