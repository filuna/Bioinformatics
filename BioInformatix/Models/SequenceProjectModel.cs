using BioInformatix.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioInformatix.Models
{
  public class SequenceProjectModel : BaseViewModel
    {
    public SequenceProject Project { get; set; }
        public string[] TaxonNames { get; set; }

    public IDictionary<int, IList<string>> LineageForView { get; set; }

  }
}