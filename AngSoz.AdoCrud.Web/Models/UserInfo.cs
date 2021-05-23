using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AngSoz.AdoCrud.Web.Models
{
    public class UserInfo
    {
        [Display(Name = "Id")]
        public int Id;

        [Required(ErrorMessage = "Username is requerid. ")]
        public string UserName { get; set; }

    }
}
