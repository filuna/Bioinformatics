using BioInformatix.Class;
using BioInformatix.Models;
using BioInformatix.Services;
using MongoBaseRepository;
using MongoBaseRepository.Classes;
using MongoDB.Bson;
using System.Linq;
using System.Web.Mvc;
using System;

namespace BioInformatix.Controllers
{
  public class BaseController : Controller
  {
    protected IBioInformatixService bioService;
    protected IMongoRepository db;
    public BaseController(IBioInformatixService _bioService = null, IMongoRepository _db = null)
    {
      bioService = _bioService ?? DependencyResolver.Current.GetService<IBioInformatixService>();
      db = _db ?? DependencyResolver.Current.GetService<IMongoRepository>();
    }

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if (filterContext.Controller.ViewData.Model == null) filterContext.Controller.ViewData.Model = new BaseViewModel();
			base.OnActionExecuted(filterContext);
		}

		protected MongoUser GetAuthUser()
    {
      if (System.Web.HttpContext.Current.User != null) return db.GetItem<MongoUser>(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name);
      return null;
    }

    protected ObjectId GetAuthUserId()
    {
      if (System.Web.HttpContext.Current.User != null)
      {
        MongoUser user = db.GetItem<MongoUser>(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name);
        if (user != null) return user._id;
      }
      return ObjectId.Empty;
    }

    protected UserProject GetActualUserProject()
    {
      MongoUser user = GetAuthUser();
      if (user != null)
      {
        UserProject up = db.GetItem<UserProject>(x => x.UserId == user._id);
        return up;
      }
      return null;
    }

    protected BaseViewModel GenerateViewModel()
    {
      BaseViewModel model = new BaseViewModel() { UserProject = GetActualUserProject() };
      return model;
    }
  }
}