using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Common.Helpers;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;
using System;

namespace MyEvernote.BusinessLayer
{
    public class EvernoteUserManager : ManagerBase<EvernoteUser>
    {
        public BusinessLayerResult<EvernoteUser> RegisterUser(RegisterViewModel data)
        {
            EvernoteUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<EvernoteUser> layerResult = new BusinessLayerResult<EvernoteUser>();

            if(user !=null)
            {
                if(user.Username == data.Username)
                {
                    layerResult.AddError(ErrorMessageCode.UsernameAlreadyExists, "Lütfen e-postanızı kontrol ediniz, Üyeliğinizi aktifleştiriniz.");
                }

                if (user.Email == data.Email)
                {
                    layerResult.AddError(ErrorMessageCode.EmailAlreadyExists, "E-Posta Adresiniz Kayıtlı");

                }

            }


            int dbResutl = base.Insert(new EvernoteUser()
            {
                Username = data.Username,
                Email = data.Email,
                Password = data.Password,
                ProfileImageFile = "user_boy.png",
                ActiveGuid = Guid.NewGuid(),
                IsActive = false,
                IsAdmin = false,

            });

            if(dbResutl > 1)
            {
                layerResult.Result = Find(x => x.Email == data.Email && x.Username == data.Username);

                string siteuri = ConfigHelper.Get<string>("SiteRootUrl");
                string activeuri = $"{siteuri}/Home/UserActivate/{layerResult.Result.ActiveGuid}";
                string body = $"Hesabınızı Aktive Etmek için <a href='{activeuri}' target='_blank'>Tıklayınız</a>";
                MailHelper.SendMail(body, layerResult.Result.Email,"Hesap Aktivasyon",true);

                //TODO: aktivasyon maili atılacak
            }

            return layerResult;

        }

        public BusinessLayerResult<EvernoteUser> GetUserById(int id)
        {

            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.Id == id);

            if(res.Result == null)
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı Bulunamadı");
            }

            return res;
         
        }

        public BusinessLayerResult<EvernoteUser> LoginUser(LoginViewModel data)
        {
            
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.Username == data.Username || x.Password == data.Password);

            if (res.Result != null)
            {
                if(!res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.ChechYourEmail,"Lütfen e-postanızı kontrol ediniz, Üyeliğinizi aktifleştiriniz.");
                }
            }

            else
            {
                res.AddError(ErrorMessageCode.UsernameorPassWrong, "Kullanıcı Adı ve Şifre Uyuşmamaktadır.");
            }

            return res;

        }

        public BusinessLayerResult<EvernoteUser> UpdateProfile(EvernoteUser data)
        {

            EvernoteUser db_user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();


            if (db_user !=null && db_user.Id != data.Id)
            {

                if(db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı Adı Kayıtlı");

                }

                if (db_user.Email == data.Email)

                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-Posta Adres Kayıtlı");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;

            if(string.IsNullOrEmpty(data.ProfileImageFile) == false)
            {

                res.Result.ProfileImageFile = data.ProfileImageFile;
            }

            if(base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.ProfileCouldNotUpdated, "Profil Güncellenemedi");
            }

            return res;


        }

        public BusinessLayerResult<EvernoteUser> RemoveUserById(int id)
        {

            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser user = Find(x => x.Id == id);

            if (user != null)
            {
                if (Delete(user) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı silinemedi.");
                    return res;
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UserCouldNotFind, "Kullanıcı bulunamadı.");
            }

            return res;



        }

        public BusinessLayerResult<EvernoteUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.ActiveGuid == activateId);

            if(res.Result != null)
            {
                if(res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyActive, "Kullanıcı Zaten Aktif Edilmiştir.");
                    return res;
                }

                res.Result.IsActive = true;
                Update(res.Result);

            }

            else
            {

                res.AddError(ErrorMessageCode.ActivateIdDoesNotExists, "Aktifleştirilecek Kullanıcı Bulunamadı");

            }


            return res;
        }

        // Method hiding..

        public new BusinessLayerResult<EvernoteUser> Insert(EvernoteUser data)
        {
            EvernoteUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            res.Result = data;

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }

                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi kayıtlı.");
                }
            }
            else
            {
                res.Result.ProfileImageFile = "user_boy.png";
                res.Result.ActiveGuid = Guid.NewGuid();

                if (base.Insert(res.Result) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı eklenemedi.");
                }
            }

            return res;
        }

        public new BusinessLayerResult<EvernoteUser> Update(EvernoteUser data)
        {
            EvernoteUser db_user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = data;

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExists, "E-posta adresi kayıtlı.");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdated, "Kullanıcı güncellenemedi.");
            }

            return res;
        }

    }
}
