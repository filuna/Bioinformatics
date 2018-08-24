using BioInformatix.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BioInformatix.Controllers
{
  [Authorize]
  public class ClustalController : BaseController
    {
        // GET: Clustal
        public ActionResult Index()
        {
            return View();
        }
    }
}