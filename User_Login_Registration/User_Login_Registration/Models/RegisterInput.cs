using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace User_Login_Registration.Models
{
    public class RegisterInput
    {
        //[Required]
        //[DataType(DataType.EmailAddress)]
        //[EmailAddress]
        public string email { get; set; }

        public string password { get; set; }

    }
}