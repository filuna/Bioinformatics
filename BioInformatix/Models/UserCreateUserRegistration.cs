using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioInformatix.Models
{
  public class UserCreateUserRegistration : BaseViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        [UIHint("LongText")]
        public string Info { get; set; }
        [UIHint("SectionDDL")]
        public int ParentSectionId { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [UIHint("Password repeat")]
        [DataType(DataType.Password)]
        public string PasswordRepeat { get; set; }

        public string Email { get; set; }
        public bool IsSection { get; set; }
        public bool IsAdmin { get; set; }
        public bool FromSection { get; set; }
    }
}
