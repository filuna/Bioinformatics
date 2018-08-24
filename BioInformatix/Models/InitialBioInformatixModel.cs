using BioInformatix.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioInformatix.Models
{
  public class InitialBioInformatixModel : BaseViewModel
  {
    public SequenceProject NewProject { get; set; }

    public IList<SequenceProject> Projects { get; set; }
  }
}