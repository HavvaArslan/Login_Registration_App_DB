using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace User_Login_Registration.Models
{
    public class RegisterOutput
    {
        public string email { get; set; }
        public string password { get; set; }
        public DateTime lastLoginTime { get; set; }
        public int stat { get; set; }
        public DateTime lastUpdateDate { get; set; }
        public int recordStat { get; set; }
        public string hashType { get; set; }
        public int blockCount { get; set; }
    }
}