using MongoBaseRepository.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BioInformatix.Class
{
    public class SequenceProject : BaseNamedEntity
    {
        [DisplayName("Popis")]
        public string Description { get; set; }
        public string BlastSearchGuid { get; set; }
        public string SvgBlast { get; set; }
        //List s různými zarovnáními celého nebo částí souboru
        public IList<AlignmentSequence> Alligments { get; set; }
        public IList<string> ActualTaxons { get; set; }
    }
}