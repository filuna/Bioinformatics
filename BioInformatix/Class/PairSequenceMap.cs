using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioInformatix.Class
{
  /// <summary>
  /// Párové zarovnání sekvencí
  /// </summary>
  public class PairSequenceMap
  {
    public ObjectId FirstId { get; set; }
    public ObjectId SecondId { get; set; }
    public string ResultSequence { get; set; }
  }
}