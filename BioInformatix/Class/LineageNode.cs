using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioInformatix.Class
{
  public class LineageNode
  {
    public string Name { get; set; }
    public int BranchNo { get; set; }
    public IList<LineageNode> Items { get; set; }
  }
}