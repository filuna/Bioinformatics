using BioInformatix.Class;
using BioInformatix.Models;
using BioInformatix.Services;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BioInformatix.Controllers
{
  [Authorize]
  public class CazyController : BaseController
  {
    private ICazyConnector connector;

    public CazyController(ICazyConnector _connector = null)
    {
      connector = _connector ?? DependencyResolver.Current.GetService<ICazyConnector>();
      bioService = bioService ?? DependencyResolver.Current.GetService<IBioInformatixService>();
    }
    // GET: Cazy
    public ActionResult Index(CazyDataModel model)
    {

      if (model == null) model = new CazyDataModel();
      UserProject up = GetActualUserProject();
      if (up != null) model.Items = bioService.GetSequenceData(up.ProjectId);
      return View(model);
    }

		[HttpPost]
    public ActionResult GetCazyData(string url, string pattern)
    {
      if (!String.IsNullOrEmpty(url))
      {
        connector.BaseUrl = url;
        connector.Pattern = pattern;
        UserProject up = GetActualUserProject();
        if (up != null)
        {
          IList<CazyItem> items = connector.GetCazyItems();
          bioService.SaveSequenceData(items, up);
					return Json(true, JsonRequestBehavior.AllowGet);
				}
				else return Json(false, JsonRequestBehavior.AllowGet);
			}
			return Json(false, JsonRequestBehavior.AllowGet);
    }
  }
}