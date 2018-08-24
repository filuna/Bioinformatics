using BioInformatix.Class;
using BioInformatix.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BioInformatix.Controllers
{
  public class HomeController : BaseController
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult GetProjectName()
    {
      UserProject up = GetActualUserProject();
      if (up != null)
      {
        return Json(up.ProjectString);
      }
      return Json(null);
    }
  }
}