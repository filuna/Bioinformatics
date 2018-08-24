using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioInformatix.Models
{
    public class TreeModel: BaseViewModel
    {
        public string TreeString { get; set; }
        public string TreeFile { get; set; }
        public string ProjectId { get; set; }
        public string AlignmentId { get; set; }
        public string AlignmentName { get; set; }
    }
}