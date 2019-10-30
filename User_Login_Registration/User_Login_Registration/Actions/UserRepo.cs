using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using User_Login_Registration.Models;

namespace User_Login_Registration.Actions
{
    public class UserRepo
    {
        private SqlConnection con;
        private SqlCommand com;

        private void connection()
        {
            //SSMS için bağlantısı için
            string conString = ConfigurationManager.ConnectionStrings["baglantim"].ConnectionString;
            con = new SqlConnection(conString);
        }

        //Yeni kullanıcı kayıt işleminin gerçekleştiği metod
        public int AddUsers(RegisterOutput regOut)
        {
            connection();
            com = new SqlCommand("sp_InsertData", con);
            com.CommandType = CommandType.StoredProcedure;
           
            com.Parameters.AddWithValue("@Password", regOut.password);
            com.Parameters.AddWithValue("@Email", regOut.email);
            com.Parameters.AddWithValue("@LastLoginTime", regOut.lastLoginTime);
            com.Parameters.AddWithValue("@Stat", regOut.stat);
            com.Parameters.AddWithValue("@LastUpdateDate", regOut.lastUpdateDate);
            com.Parameters.AddWithValue("@RecordStat", regOut.recordStat);
            com.Parameters.AddWithValue("@HashType", regOut.hashType);
            com.Parameters.AddWithValue("@BlockCount", regOut.blockCount);
            con.Open();
            int i = com.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                return 1;
            }
            return 0;
            
        }

        public int CheckUser(string email)
        {
            //Yeni kullanıcı kaydı sırasında aynı email değerine sahip başka bir kullanıcı
            //olup olmadığını kontrol eden metod
            connection();
            com = new SqlCommand("sp_checkUser", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Email", email);
            com.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;
            con.Open();
            com.ExecuteNonQuery();
            int i = int.Parse(com.Parameters["@result"].Value.ToString());
            con.Close();
            return i;
        }

        public int Login(Login Log)
        {
            int block_count=0;
            connection();
            com = new SqlCommand("sp_login", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Password", Log.password);
            com.Parameters.AddWithValue("@Email", Log.email);
            com.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;
            con.Open();
            com.ExecuteNonQuery();
            //result -> SSMS tarafında, gönderilen verilere göre dönen değer
            //Email ve şifrenin eşleşmesi durumunda result değeri 1
            //Şifrenin yanlış girilmesi durumunda result değeri 2
            //Sisteme kayıtlı olmayan bir email girilmesi durumunda result değeri 4
            //Diğer durumlarda result 0 değerini alacaktır

            int i = int.Parse(com.Parameters["@result"].Value.ToString());
            if (i != 4)
            {
                //block_count -> Email veri tabanında kayıtlı ise ilgili emaile ait vt'de tutulan bloke sayısı
                block_count = BlockCount(Log.email);
            }

            if (block_count == 3)
            {
                //block_count değeri 3 ise(3 defa yanlış şifre girilme işlemi yapıldıysa)
                //Kullanıcının bloke olması
                Block(Log.email);
                return 3;
            }
            else if (i == 2)    //şifre yanlış girildiyse
            {
                //Veri tabanında bloke sayısını arttırır
                BlockControl(Log.email);
            }
            con.Close();
            return i;
        }

        //parametre olarak gönderilen emaile ait bloke sayısı
        public int BlockCount(string email)
        {
            connection();
            com = new SqlCommand("sp_block_control", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Email", email);
            com.Parameters.Add("@BlockControl", SqlDbType.Int).Direction = ParameterDirection.Output;
            con.Open();
            com.ExecuteNonQuery();
            int i = int.Parse(com.Parameters["@BlockControl"].Value.ToString());
            con.Close();
            return i;
        }

        //Yanlış şifre girme işlemi gerçekleştirildikçe veritabanında ilgili
        //alanın bloke sayını arttırır
        public void BlockControl(string email)
        {
            connection();
            com = new SqlCommand("sp_block", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Email", email);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }


        //3 kez hatalı şifre girilmesi durumunda kullanıcının bloke olması
        //işlemini gerçekleştiren metod
        public void Block(string email)
        {
            connection();
            com = new SqlCommand("sp_change_stat", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Email", email);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }
    }
}