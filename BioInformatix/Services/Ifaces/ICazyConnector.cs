using BioInformatix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioInformatix.Services
{
  public interface ICazyConnector
  {
        string BaseUrl { get; set; }
        string Pattern { get; set; }
        IList<CazyItem> GetCazyItems(string url = null, bool ismain = true);
  }
}
