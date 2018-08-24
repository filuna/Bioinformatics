using MongoDB.Bson;
using MongoBaseRepository;
using MongoBaseRepository.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using BioInformatix.Models;

namespace BioInformatix.Controllers
{
    public partial class AccountController: Controller
    {
        protected readonly IMongoRepository Db;
        protected readonly IMongoMembershipService MemberShip;
        protected ObjectId AuthUserId
        {
          get
          {
            MongoUser user = GetAuthUser();
            return user != null ? user._id : ObjectId.Empty;
          }
        }

        protected MongoUser GetAuthUser()
        {
          if (System.Web.HttpContext.Current.User != null) return Db.GetItem<MongoUser>(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name);
          return null;
        }

        public AccountController(IMongoRepository db = null, IMongoMembershipService _memb = null)
        {
          Db = db ?? DependencyResolver.Current.GetService<IMongoRepository>();
          MemberShip = _memb ?? DependencyResolver.Current.GetService<IMongoMembershipService>();
          if (Db.GetItem<MongoUser>(x=> x.Email == "husak.ondra@gmail.com") == null) Db.AddItem<MongoUser>(new MongoUser() { Email = "husak.ondra@gmail.com", FirstName = "Ondra", SecondName = "Husák", IsApproved = true, Password = "gudath", UserName = "Gothmog" }, ObjectId.Empty);
          if (Db.GetItem<MongoUser>(x => x.Email == "bioinformatix@gmail.com") == null) Db.AddItem<MongoUser>(new MongoUser() { Email = "bioinformatix@gmail.com", FirstName = "TEST_USER", SecondName = "BIOINFORMATIKA", IsApproved = true, Password = "password", UserName = "test_user" }, ObjectId.Empty);
        }

        [HttpGet]
        public virtual ActionResult LogOn()
        {
          return View(new LoginViewModel() { });
        }

        [HttpPost]
        public virtual ActionResult LogOn(int? id)
        {
            LoginViewModel lvm = new LoginViewModel();
            TryUpdateModel<LoginViewModel>(lvm);
            if (MemberShip.ValidateUser(lvm.Password, lvm.LoginName))
            {
                FormsAuthentication.SetAuthCookie(lvm.LoginName, true);
                return RedirectToAction("Index", "Home");
            }
            else return RedirectToAction("ErrLogOn");
        }

        public virtual ActionResult ErrLogOn()
        {
            return View();
        }

        public virtual ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public MongoUser GetUserFromModel(UserCreateUserRegistration model)
        {
          MongoUser user = new MongoUser();
          return user;
        }

        
        public ActionResult DeleteUser(string username, ObjectId id)
        {
          if (String.IsNullOrEmpty(username))
          {
            Db.RemoveItem<MongoUser>(x=> x._id == id);
          }
          else Db.RemoveItem<MongoUser>(x => x.UserName == username);
            return RedirectToAction("Index", "Home");
        }
    }
}
