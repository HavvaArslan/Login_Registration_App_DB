using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using User_Login_Registration.Models;
using User_Login_Registration.Actions;
using System.Web.Http.Cors;
using System.Security.Cryptography;
using System.Text;

namespace User_Login_Registration.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        
        static UserRepo repository = new UserRepo();
        Hash hashing = new Hash();
        string hash = "havva";
      //  static string decryptedData;
        static string encryptedData;


        [Route("api/user/CreateUser")]
        [HttpPost]
        public object CreateUser(RegisterInput reg)
        {
           
            //Girilen emaile ait kullanıcı mevcut mu kontrolünü yapar
            var response_check = repository.CheckUser(reg.email);

            if (response_check != 1)    //Aynı email adresine ait kullanıcı yoksa kayıt işlemini gerçekleştir
            {
                RegisterOutput regOut = new RegisterOutput();

                //Kullanıcıdan alınan şifreyi md5 ile hashing işlemi
                encryptedData = hashing.Encrypt(reg.password,hash); 
              //  decryptedData = Decrypt(encryptedData, hash);
                

                DateTime date = DateTime.UtcNow;
                regOut.email= reg.email;
                regOut.password = encryptedData;
                regOut.hashType = "md5";    //Seçilen hashing algoritması
                regOut.lastLoginTime = date;
                regOut.lastUpdateDate = date;
                regOut.recordStat = 1; //Kullanıcı kaydı yeni oluşturulduğu için default olarak 1(aktif) değerini alır
                regOut.stat = 1; //Kullanıcı kaydı yeni oluşturulduğu için stat default olarak 1(aktif) değerini alır
                regOut.blockCount = 0; //Yeni oluşturulmuş bir kullanıcı kaydı olduğundan veri tabanında henüz bir bloke işlemi yoktur. Default olarak 0 değerini alır

                    var response = repository.AddUsers(regOut);
                    
                    if (response <= 0)
                    {
                        return new Response
                        { Status = "Error", Message = "Failed to Create Record" };
                    }

                   return new Response
                    { Status = "Success", Message = "Successfully Saved." };

            }
            else
            {
                return new Response
                { Status = "Error", Message = "Email must be unique" };
            }

            
        }

        [Route("api/user/Login")]
        [HttpPost]
        public object Login(Login Log)
        {
            Log.password = hashing.Encrypt(Log.password,hash);
            var result = repository.Login(Log);
            if (result == 0 || result==2)
            {
                return new Response
                { Status = "Error", Message = "Failed." };
            }
            else if (result == 3)
            {
                return new Response
                { Status = "Error", Message = "Account Blocked." };
            }
            else if (result == 4)
            {
                return new Response
                { Status = "Error", Message = "Invalid Email." };
            }
            return new Response
            { Status = "Success", Message = "Login successful." };
        }

        
       
    }
}

