using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities
{
    [Table("EvernoteUser")]
    public class EvernoteUser : MyEntityBase
    {
        [StringLength(300)]
        public string Name { get; set; }

        [StringLength(300)]
        public string Surname { get; set; }

        [Required,StringLength(300)]
        public string Username { get; set; }

        [Required, StringLength(300)]
        public string Email { get; set; }

        [Required,StringLength(300)]
        public string Password { get; set; }

        [StringLength(100), ScaffoldColumn(false)] //images/user_12.jpeg
        public string ProfileImageFile { get; set; }

        public bool IsActive { get; set; }

        [Required, ScaffoldColumn(false)]
        public Guid ActiveGuid { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        public virtual List<Note> Notes { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Liked> Likes { get; set; }
    }
}
