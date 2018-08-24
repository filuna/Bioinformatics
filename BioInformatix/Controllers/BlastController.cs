using BioInformatix.Class;
using BioInformatix.Models.BlastModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BioInformatix.Controllers
{
	[Authorize]
	public class BlastController : BaseController
	{

		// GET: Blast
		public ActionResult Index()
		{
			BlastViewModel model = new BlastViewModel() { Parameters = new Dictionary<string, IList<SelectListItem>>() };
			model.Init(bioService);
			var up = GetActualUserProject();
			model.ProjectId = up != null ? up.ProjectId.ToString() : null;
			if (model.ProjectId != null)
			{
				model.Project = db.GetItem<SequenceProject>(x => x._id == up.ProjectId);
				model.LineageForView = bioService.GetLineageForView(up.ProjectId, true);
				return View(model);
			}
			else return RedirectToAction("Index", "Project");
		}

		public ActionResult BlastSearch(string err = null)
		{
			BlastViewModel model = new BlastViewModel() { Parameters = new Dictionary<string, IList<SelectListItem>>(), ErrorMessage = err };
			model.Init(bioService);
			var up = GetActualUserProject();
			model.ProjectId = up != null ? up.ProjectId.ToString() : null;
			if (model.ProjectId != null)
			{
				model.Project = db.GetItem<SequenceProject>(x => x._id == up.ProjectId);
				return View(model);
			}
			else return RedirectToAction("Index", "Blast");
		}

		[HttpPost]
		public ActionResult RunBlastSearch(BlastViewModel model, FormCollection col)
		{
			if (model != null)
			{
				ObjectId projId = ObjectId.Parse(model.ProjectId);
				try
				{
					bioService.GenerateBlastSearch(projId, model);
				}
				catch (Exception ex)
				{
					return RedirectToAction("BlastSearch", new { err = ex.Message });
				}
				model.Init(bioService);
				model.Project = db.GetItem<SequenceProject>(x => x._id == projId);
				model.IsRedirect = true;
				return View("BlastSearch", model);
			}
			return RedirectToAction("Index", "Blast");
		}

		public ActionResult GetBlastSearchResult(string projectId)
		{
			ObjectId projId = ObjectId.Parse(projectId);
			SequenceProject project = db.GetItem<SequenceProject>(x => x._id == projId);
			return Json(bioService.GetBlastSearchResult(project, Server.MapPath("~/ServiceResults/"), GetActualUserProject()));
		}
	}
}