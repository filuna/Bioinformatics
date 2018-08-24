using BioInformatix.Class;
using BioInformatix.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace BioInformatix.Controllers
{
  [Authorize]
  public class ProjectController : BaseController
  {
    public ActionResult Index()
    {
      InitialBioInformatixModel model = new InitialBioInformatixModel() { NewProject = new Class.SequenceProject(), Projects = bioService.GetProjects(GetAuthUserId()) };
      return View(model);
    }

    public ActionResult ProjectErr()
    {
      return View();
    }

    [HttpPost]
    public ActionResult SelectSequeceForProject([DataSourceRequest]DataSourceRequest request, string projectId, string[] taxons = null, bool isBlast = false)
    {
      DataSourceResult result = bioService.GetSequenceForProject(request, projectId, taxons, isBlast);
      return this.Json(result, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public ActionResult DeleteSequenceForProject(string projectId, bool isBlast = false)
    {
      bioService.DeleteSequencesForProject(projectId, isBlast);
      return this.Json(true, JsonRequestBehavior.AllowGet);
    }

    public bool ChangeMarkedSequenceSelection(string projectId, string[] taxons, bool isBlast = false)
    {
      bioService.SelUnSelAllSequence(projectId, false, isBlast);
      List<Sequence> sequences = bioService.GetSequenceForProject(projectId, taxons, isBlast).ToList();
      sequences.ForEach(x => x.Selected = true);
      bioService.UpdateSequenceData(sequences);
      return true;
    }

    public bool UnselectSequenceSelection(string projectId, bool isBlast = false)
    {
      bioService.SelUnSelAllSequence(projectId, false, isBlast);
      return true;
    }

    public bool ChangeSequenceSelection(string seqId)
    {
      Sequence seq = bioService.GetSequence(seqId);
      seq.Selected = !seq.Selected;
      bioService.UpdateSequence(seq);
      return true;
    }


    [HttpPost]
    public string GetLineage()
    {
      LineageNode lineageTree = bioService.GetLineageTree(GetActualUserProject().ProjectId);
      var list = Newtonsoft.Json.JsonConvert.SerializeObject(lineageTree);
      return list;
    }

    public ActionResult ProjectEdit(string projectId)
    {
      SequenceProjectModel model = new SequenceProjectModel();
      if (!String.IsNullOrEmpty(projectId))
      {
        ObjectId projId = ObjectId.Parse(projectId);
        model.Project = db.GetItem<SequenceProject>(x => x._id == projId);
        model.LineageForView = bioService.GetLineageForView(projId, false);
        ViewBag.TaxonNames = model.Project.ActualTaxons;
        return View(model);
      }
      return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult ActualizeUserProject(string projectId)
    {
      UserProject up = GetActualUserProject();
      ObjectId projId = ObjectId.Parse(projectId);
      if (up != null)
      {
        up.ProjectId = projId;
        up.ProjectString = db.GetItem<SequenceProject>(x => x._id == projId).Name;
        db.UpdateItem(up);
      }
      else
      {
        up = new UserProject();
        up.ProjectId = projId;
        up.ProjectString = db.GetItem<SequenceProject>(x => x._id == projId).Name;
        up.UserId = GetAuthUser()._id;
        db.AddItem(up, up.UserId);
      }
      return RedirectToAction("Index", "Home");
    }

    public ActionResult SaveProject(InitialBioInformatixModel model)
    {
      bioService.SaveProject(model.NewProject, GetAuthUserId());
      ActualizeUserProject(model.NewProject.IdString);
      return RedirectToAction("Index", "Project");
    }
  }
}