using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioInformatix.Models
{
    public class CazyItem
    {
        public string ProteinName { get; set; }
        public string ProteinShortCut { get; set; }
        public string Organism { get; set; }
        public string UniprotUrl { get; set; }
        public string FastaProtein { get; set; }
        public bool Selected { get; set; }
        public ObjectId DbIndex { get; set; }
        public IList<string> Lineage { get; set; }

    }
}