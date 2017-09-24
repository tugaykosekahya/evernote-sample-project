using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyEvernote.Entities.ValueObjects
{
    public class LoginViewModel
    {
        [DisplayName("Kullanıcı Adı"), Required(ErrorMessage = "{0} alanı boş geçilemez"), StringLength(25,ErrorMessage = "Kullanıcı Adı En Fazla 25 Karakter Olmalıdır.")]
        public string Username { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı boş geçilemez"),DataType(DataType.Password), StringLength(25, ErrorMessage = "Şifre En Fazla 25 Karakter Olmalıdır.")]
        public string Password { get; set; }
    }
}