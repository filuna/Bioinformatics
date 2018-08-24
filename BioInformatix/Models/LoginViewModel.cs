using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BioInformatix.Models
{
    public class LoginViewModel: BaseViewModel
    {
        [DisplayName("Login")]
        [Required]
        public string LoginName { get; set; }
        [DisplayName("Heslo")]
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}