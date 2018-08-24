using BioInformatix.Class;
using BioInformatix.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BioInformatix.Controllers
{
  [Authorize]
  public class TreeController : BaseController
    {
    public ActionResult GetTreeResult(string projectId, string alignmentId)
    {
      ObjectId projId = ObjectId.Parse(projectId);
      SequenceProject project = db.GetItem<SequenceProject>(x => x._id == projId);
      return Json(bioService.GetSimplePhyloResult(project, alignmentId));
    }

    public ActionResult ViewTree(string projectId, string alignmentId)
    {
      ObjectId projId = ObjectId.Parse(projectId);
      SequenceProject project = db.GetItem<SequenceProject>(x => x._id == projId);
      if (project != null && alignmentId != String.Empty)
      {
        AlignmentSequence alignment = project.Alligments.First(x => x.IdString == alignmentId);
        if (alignment != null && !String.IsNullOrEmpty(alignment.PhylogeneticTree))
        {
                    string fileContent = String.Empty;
                    if (System.IO.File.Exists(alignment.TreePicture))
                    {
                        using (StreamReader sr = new StreamReader(alignment.TreePicture))
                        {
                            fileContent = sr.ReadToEnd();
                        }
                    }
                    return View("ViewTree",new TreeModel() { TreeString = alignment.PhylogeneticTree, AlignmentName = alignment.Description, ProjectId = projectId, AlignmentId = alignmentId, TreeFile = fileContent });
        }
      }
      return View();
    }

        public ActionResult AddTreePicture(FormCollection col, TreeModel model)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);

                    var path = Path.Combine(Server.MapPath("~/ServiceResults/Trees/"), fileName);
                    file.SaveAs(path);
                    ObjectId projId = ObjectId.Parse(model.ProjectId);
                    SequenceProject project = db.GetItem<SequenceProject>(x => x._id == projId);
                    if (project != null && model.AlignmentId != String.Empty)
                    {
                        AlignmentSequence alignment = project.Alligments.First(x => x.IdString == model.AlignmentId);
                        if (alignment != null)
                        {
                            alignment.TreePicture = path;
                            db.UpdateItem(project);
                        }

                    }
                }
            }
            return RedirectToAction("ViewTree", new { projectId = model.ProjectId, alignmentId = model.AlignmentId });
        }

        public ActionResult AddTree(string projectId, string alignmentId)
    {
      bioService.GenerateTree(ObjectId.Parse(projectId), alignmentId, Server.MapPath("~/ServiceResults/"));
      return Json(true);
    }
  }
}