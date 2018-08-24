using BioInformatix.Class;
using BioInformatix.Models;
using Kendo.Mvc.UI;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BioInformatix.Controllers
{
  [Authorize]
  public class AlignmentController : BaseController
    {
    public ActionResult GetAlignmentResult(string projectId, string alignmentId)
    {
      ObjectId projId = ObjectId.Parse(projectId);
      SequenceProject project = db.GetItem<SequenceProject>(x => x._id == projId);
      return Json(bioService.GetClustalResult(project, alignmentId));
    }

    public ActionResult ViewAlignment(string projectId, string alignmentId)
    {
      ObjectId projId = ObjectId.Parse(projectId);
      SequenceProject project = db.GetItem<SequenceProject>(x => x._id == projId);
      if (project != null && alignmentId != String.Empty)
      {
        AlignmentSequence alignment = project.Alligments.First(x => x.IdString == alignmentId);
        if (alignment != null && alignment.AllignedSequences != null && alignment.AllignedSequences.Any())
        {
          return View("ViewAlignmentRaw", new AlignmentModel() { AlignmentSequences = alignment.AllignedSequences, AlignmentName = alignment.Description, AlignmentId = alignmentId, ProjectId = projectId });
        }
      }
      return View(new HtmlString(""));
    }

    public ActionResult AddAligment(string projectId, string title)
    {
      bioService.GenerateClustalAligment(ObjectId.Parse(projectId), title, Server.MapPath("~/ServiceResults/"));
      return Json(true);
    }


        public ActionResult SelectAlignmentForProject([DataSourceRequest]DataSourceRequest request, string projectId)
    {
      DataSourceResult result = bioService.GetAlignmentForProject(request, projectId);
      return this.Json(result, JsonRequestBehavior.AllowGet);
    }
  }
}