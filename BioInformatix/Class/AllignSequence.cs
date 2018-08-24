using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioInformatix.Class
{
    /// <summary>
    /// Jeden řádek zarovnání
    /// </summary>
    public class AllignSequence
    {
        public string AlignmentGuid { get; set; }
        public string PhylogeneticGuid { get; set; }
        public string PhylogeneticTree { get; set; }
        public string Description { get; set; }
        public bool IsFinished { get; set; }
        public IList<AlignRecord> AllignedSequences { get; set; }
        public string Filename { get; set; }
    }

  public class AlignRecord
  {
    public string SequenceName { get; set; }
    public ObjectId SequenceId { get; set; }
    public string AlignedSequence { get; set; }
  }
}