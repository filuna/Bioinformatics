using BioInformatix.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BioInformatix.Models
{
    public class CazyDataModel : BaseViewModel
    {
        [DisplayName("Adresa")]
        public string Url { get; set; }
        [DisplayName("Text k vyhledávání")]
        public string Pattern { get; set; }
        public IList<CazyItem> Items { get; set; }
    }
}