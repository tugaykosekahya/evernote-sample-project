using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyEvernote.Entities.ValueObjects
{
    public class RegisterViewModel
    {
        [DisplayName("Kullanıcı Adı"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Username { get; set; }

        [DisplayName("Email"), Required(ErrorMessage = "{0} alanı boş geçilemez"), DataType(DataType.Password), StringLength(75, ErrorMessage = "Şifre En Fazla 70 Karakter Olmalıdır."), EmailAddress(ErrorMessage = "{0} alanı için geçerli bir e-posta giriniz.")]
        public string Email { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı boş geçilemez"), DataType(DataType.Password), StringLength(25, ErrorMessage = "Şifre En Fazla 25 Karakter Olmalıdır.")]
        public string Password { get; set; }

        [DisplayName("Şifre Tekrar"), Required(ErrorMessage = "{0} alanı boş geçilemez"), DataType(DataType.Password), StringLength(25, ErrorMessage = "Şifre En Fazla 25 Karakter Olmalıdır."),Compare("Password",ErrorMessage = "Şifre ile Şifre Tekrar Uyuşmamaktadır.")]
        public string RePassword{ get; set; }



    }
}