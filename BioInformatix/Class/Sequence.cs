using MongoBaseRepository.Classes;
using BioInformatix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BioInformatix.Class
{
  public class Sequence : BaseNamedEntity
  {
    public Sequence()
    {

    }

    public Sequence(CazyItem item)
    {
      this.Organism = item.Organism;
      this.Name = item.ProteinName;
      this.FastaUrl = item.UniprotUrl;
      this.FastaProtein = item.FastaProtein;
      this.Lineage = item.Lineage;
    }

    public CazyItem GetItemFromSequence()
    {
      CazyItem item = new CazyItem();
      item.Lineage = this.Lineage;
      item.UniprotUrl = this.FastaUrl;
      item.Organism = this.Organism;
      item.ProteinName = this.Name;
      return item;
    }
    public ObjectId ProjectId { get; set; }
    public string ProteinShortCut { get; set; }
    public string Organism { get; set; }
    public string FastaUrl { get; set; }
    public string FastaProtein { get; set; }
    public string Description { get; set; }
    public Guid BlastSearchGuid { get; set; }
    public string BlastAlignedSequence { get; set; }
    public bool Selected { get; set; }
    public IList<string> Lineage { get; set; }
  }
}