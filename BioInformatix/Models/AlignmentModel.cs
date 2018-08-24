using BioInformatix.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioInformatix.Models
{
  public class AlignmentModel: BaseViewModel
  {
    public IList<AlignRecord> AlignmentSequences { get; set; }
    public string ProjectId { get; set; }
    public string AlignmentId { get; set; }
    public string AlignmentName { get; set; }
  }
}