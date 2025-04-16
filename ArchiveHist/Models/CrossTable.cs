using System.Collections.Generic;

namespace ArchiveHist.Models
{
    public class CrossTable
    {
        public CrossTable()
        {
            AudioFiles = new List<AudioFile>();
            Collections = new List<Collection>();
            Delanceys = new List<Delancey>();
            Maps = new List<Map>();
            Oversizeds = new List<Oversized>();
            Photos = new List<Photo>();
            PoisonBooks = new List<PoisonBook>();
            ReportsPubs = new List<ReportsPub>();
            Researches = new List<Research>();
            Trunks = new List<Trunk>();
        }

        public List<AudioFile> AudioFiles { get; set; }
        public List<Collection> Collections { get; set; }
        public List<Delancey> Delanceys { get; set; }
        public List<Map> Maps { get; set; }
        public List<Oversized> Oversizeds { get; set; }
        public List<Photo> Photos { get; set; }
        public List<PoisonBook> PoisonBooks { get; set; }
        public List<ReportsPub> ReportsPubs { get; set; }
        public List<Research> Researches { get; set; }
        public List<Trunk> Trunks { get; set; }

        public int TotalCount { get; set; }
    }
}
