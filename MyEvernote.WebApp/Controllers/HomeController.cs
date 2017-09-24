using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using MyEvernote.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Controllers
{

    [Exc]
    public class HomeController : Controller
    {

        private NoteManager noteManager = new NoteManager();
        private CategoryManager categoryManager = new CategoryManager();
        private EvernoteUserManager evernoteUserManager = new EvernoteUserManager();


        // GET: Home
        public ActionResult Index()
        {
            //if (TempData["mm"] != null)
            //{
            //    return View(TempData["mm"] as List<Note>);
            //} 

            NoteManager nm = new NoteManager();
            return View(nm.ListQueryable().Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());

            //return View(nm.GetAllNoteQueryable().OrderByDescending(x => x.ModifiedOn).ToList());


        }

        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CategoryManager cm = new CategoryManager();
            Category cat = cm.Find(x=> x.Id == id.Value);

            if (cat == null)
            {
                return HttpNotFound();
            }

            //TempData["mm"] = cat.Notes;

            return View("Index",cat.Notes.Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult MostLiked()
        {
            NoteManager nm = new NoteManager();
            return View("Index", nm.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        [Auth]
        public ActionResult ShowProfile()
        {

            EvernoteUser currentUser = Session["login"] as EvernoteUser;

            EvernoteUserManager eum = new EvernoteUserManager();

            BusinessLayerResult<EvernoteUser> res = eum.GetUserById(currentUser.Id);

            if(res.Errors.Count > 0)
            {
                // Kullanıcıyı Hata Ekranına Yönlendiricez 

                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {

                    Title = "Hata Oluştu",
                    Items = res.Errors

                };


            }


            return View(res.Result);
        }

        [Auth]
        [HttpPost]
        public ActionResult EditProfile(EvernoteUser model, HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {

                if (ProfileImage != null &&

                  (ProfileImage.ContentType == "image/jpeg" ||
                    ProfileImage.ContentType == "image/jpg" ||
                    ProfileImage.ContentType == "image/png"))

                {
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";

                    ProfileImage.SaveAs(Server.MapPath($"~/Images/{filename}"));
                    model.ProfileImageFile = filename;
                }

                EvernoteUserManager eum = new EvernoteUserManager();

                BusinessLayerResult<EvernoteUser> res = eum.UpdateProfile(model);

                if (res.Errors.Count > 0)
                {
                    // Kullanıcıyı Hata Ekranına Yönlendiricez 

                    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    {

                        Title = "Profil Güncellenemedi",
                        Items = res.Errors,
                        RedirectUrl = "/Home/EditProfile"

                    };

                    return View("Error", errorNotifyObj);

                }

                CurrentSession.Set<EvernoteUser>("login", res.Result); // Profil Güncellendigi icin session güncellendi.

                return RedirectToAction("ShowProfile");


            }

            else
            {
                return View(model);
            }


        }

        [Auth]
        public ActionResult EditProfile()
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                // Kullanıcıyı Hata Ekranına Yönlendiricez 
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {

                    Title = "Hata Oluştu",
                    Items = res.Errors

                };

            }

            return View(res.Result);
        }

        [Auth]
        public ActionResult DeleteProfile()
        {
            EvernoteUser currentUser = Session["login"] as EvernoteUser;

            BusinessLayerResult<EvernoteUser> res =
                evernoteUserManager.RemoveUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Items = res.Errors,
                    Title = "Profil Silinemedi.",                   
                };

                return View("Error", errorNotifyObj);
            }

            Session.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult TestNotify()
        {
            OkViewModel model = new OkViewModel()

            {

                Header = "Yönlendirme",
                Title = "OK TEST",
                RedirectingTimeout = 3000,
                Items = new List<string>() { "Test Başarılı1" }

            };


            return View("Ok",model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {

            if(ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.LoginUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    if(res.Errors.Find( x => x.Code == ErrorMessageCode.UserIsNotActive) != null )

                    {
                        ViewBag.SetLink = "http://Home/Active/1234-abcd";

                    }

                    return View(model);
                }

                CurrentSession.Set<EvernoteUser>("login", res.Result); // Session'a kullanıcı bilgi saklama..
                return RedirectToAction("Index");   // yönlendirme..
            }




            return View(model);
        }

        public ActionResult Register()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {

                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.RegisterUser(model);

                if (res.Errors.Count > 0)

                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                }
                    

                //EvernoteUser user = null;

                //try
                //{

                //    user = eum.RegisterUser(model);

                //}
                //catch (Exception ex)
                //{
                //    ModelState.AddModelError("", ex.Message);                   
                //}


                //bool hasError = false;

                //if(model.Username == "aaa")
                //{
                //    ModelState.AddModelError("", "Bu Kullanıcı Adı Kullanılmaktadır.");
                //    hasError = true;
                //}

                //if(model.Email == "aaa@aa.com")
                //{
                //    ModelState.AddModelError("", "Bu E-Mail Kullanılmaktadır.");
                //    hasError = true;
                //}

                //if(hasError == true)
                //{
                //    return View(model);
                //}

                return RedirectToAction("RegisterOk");
            }


            return View(model);

        }

        public ActionResult RegisterOk()
        {
            return View();
        }

        public ActionResult UserActivate(Guid id)
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.ActivateUser(id);

            if (res.Errors.Count > 0)
            {

                TempData["errors"] = res.Errors;
                return RedirectToAction("UserActivateCancel");

            }

            else
            {

             return RedirectToAction("UserActivateOk");

            }
        
        }

        public ActionResult UserActivateCancel()
        {
            List<ErrorMessageObj> errors = null;

            if (TempData["errors"] !=null)
            {
               errors = TempData["errors"] as List<ErrorMessageObj>;
            }

            return View();
        }

        public ActionResult UserActivateOk()
        {

            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }

    }
}