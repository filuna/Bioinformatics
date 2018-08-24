using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioInformatix.Models.BlastModels
{
  public class BlastViewModelResult: BaseViewModel
  {
    public string Guid { get; set; }
    public string ProjectId { get; set; }
  }
}