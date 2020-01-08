using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebREST.Models;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace WebREST.Services
{
    public class ContactRepository
    {
        private const string CacheKey = "ContactStore";
        string cs = @"server=localhost;userid=root;password=root;database=cempaka";

        private List<Contact> ConnectDB()
        {
            List<Contact> users = new List<Contact>();
            var con = new MySqlConnection(cs);
            con.Open();

            var stm = "SELECT * FROM users";
            var cmd = new MySqlCommand(stm, con);

            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Contact user = new Contact();
                user.Id = Convert.ToInt32(rdr["id"].ToString());
                user.Email = rdr["emailAddress"].ToString();
                user.Name = rdr["userName"].ToString();
                user.Password = rdr["password"].ToString();
                user.CreatedDate = rdr["createdDate"].ToString();
                user.UpdateDate = rdr["updateDate"].ToString();

                users.Add(user);
            }
            con.Close();
            return users;
        }

        private byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private List<Contact> InsertDB(Contact userContact)
        {
            int id = 0;
            List<Contact> users = new List<Contact>();
            var con = new MySqlConnection(cs);
            con.Open();

            var stm = "SELECT MAX(id) as id from users";
            var cmd = new MySqlCommand(stm, con);

            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                id = Convert.ToInt32(rdr["id"].ToString());
            }
            id++;
            con.Close();
            con.Open();
            string pswd = GetHashString(userContact.Password);
            stm = "INSERT INTO users(id,emailAddress,username,password,createdDate,updateDate) VALUES("+id+",'"+userContact.Email+"','"+ userContact.Name+ "','"+ pswd + "',"+DateTime.Today.ToString("yyyyMMddHHmmss")+", "+DateTime.Today.ToString("yyyyMMddHHmmss") + ")";
            cmd = new MySqlCommand(stm, con);
            cmd.ExecuteNonQuery();

            con.Close();
            con.Open();
            stm = "SELECT * FROM users";
            cmd = new MySqlCommand(stm, con);

            MySqlDataReader rdr2 = cmd.ExecuteReader();

            while (rdr2.Read())
            {
                Contact user = new Contact();
                user.Id = Convert.ToInt32(rdr2["id"].ToString());
                user.Email = rdr2["emailAddress"].ToString();
                user.Name = rdr2["userName"].ToString();
                user.Password = rdr2["password"].ToString();
                user.CreatedDate = rdr2["createdDate"].ToString();
                user.UpdateDate = rdr2["updateDate"].ToString();

                users.Add(user);
            }

            con.Close();
            return users;
        }

        private bool CheckPswdDB(string email,string password)
        {
            List<Contact> users = new List<Contact>();
            var con = new MySqlConnection(cs);
            con.Open();

            string pswd = GetHashString(password);

            var stm = "SELECT * FROM users where emailAddress = '"+email+"' and password ='"+pswd+"'";
            var cmd = new MySqlCommand(stm, con);

            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                return true;
            }
            con.Close();

            return false;
        }

        private List<Contact> UpdateDB(Contact userContact)
        {
            List<Contact> users = new List<Contact>();
            var con = new MySqlConnection(cs);
            con.Open();

            string pswd = GetHashString(userContact.Password);
            var stm = "UPDATE users set password ='" + pswd + "',updateDate = " + DateTime.Today.ToString("yyyyMMddHHmmss") + " where id=" + userContact.Id;
            
            var cmd = new MySqlCommand(stm, con);
            cmd.ExecuteNonQuery();

            con.Close();
            con.Open();
            stm = "SELECT * FROM users";
            cmd = new MySqlCommand(stm, con);

            MySqlDataReader rdr2 = cmd.ExecuteReader();

            while (rdr2.Read())
            {
                Contact user = new Contact();
                user.Id = Convert.ToInt32(rdr2["id"].ToString());
                user.Email = rdr2["emailAddress"].ToString();
                user.Name = rdr2["userName"].ToString();
                user.Password = rdr2["password"].ToString();
                user.CreatedDate = rdr2["createdDate"].ToString();
                user.UpdateDate = rdr2["updateDate"].ToString();

                users.Add(user);
            }

            con.Close();
            return users;
        }

        private List<Contact> DeleteDB(string email)
        {
            List<Contact> users = new List<Contact>();
            var con = new MySqlConnection(cs);
            con.Open();

            var stm = "DELETE from users where emailAddress = '"+email+"'";
            var cmd = new MySqlCommand(stm, con);
            cmd.ExecuteNonQuery();

            con.Close();
            con.Open();
            stm = "SELECT * FROM users";
            cmd = new MySqlCommand(stm, con);

            MySqlDataReader rdr2 = cmd.ExecuteReader();

            while (rdr2.Read())
            {
                Contact user = new Contact();
                user.Id = Convert.ToInt32(rdr2["id"].ToString());
                user.Email = rdr2["emailAddress"].ToString();
                user.Name = rdr2["userName"].ToString();
                user.Password = rdr2["password"].ToString();
                user.CreatedDate = rdr2["createdDate"].ToString();
                user.UpdateDate = rdr2["updateDate"].ToString();

                users.Add(user);
            }

            con.Close();
            return users;
        }

        public ContactRepository()
        {
            var ctx = HttpContext.Current;
            
            if (ctx != null)
            {
                if (ctx.Cache[CacheKey] == null)
                {
                    var contacts = new Contact[]
                    {
                        new Contact
                        {
                            Id = 1, Name = "Glenn"
                        },
                        new Contact
                        {
                            Id = 2, Name = "Dan"
                        }
                    };

                    //ctx.Cache[CacheKey] = contacts;
                    ctx.Cache[CacheKey] = ConnectDB();
                }
            }
        }

        public List<Contact> GetAllContacts()
        {
            var ctx = HttpContext.Current;

            if (ctx != null)
            {
                //return (Contact[])ctx.Cache[CacheKey];
                return ConnectDB();
            }

            return null;
            //return new Contact[]
            //    {
            //        new Contact
            //        {
            //            Id = 0,
            //            Name = "Placeholder"
            //        }
            //    };
        }

        public bool LoginCheck(string email,string password)
        {
            return CheckPswdDB(email,password);
        }

        public bool DeleteContacts(string email)
        {
            var ctx = HttpContext.Current;
            
            if (ctx != null)
            {
                try
                {
                    //var currentData = ((List<Contact>)ctx.Cache[CacheKey]).ToList();
                    var currentData = ConnectDB();

                    foreach (Contact item in currentData)
                    {
                        if (item.Email.Equals(email))
                        {
                            ctx.Cache[CacheKey] = DeleteDB(email);
                            return true;
                        }
                    }
                    ctx.Cache[CacheKey] = currentData.ToArray();

                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return false;
        }

        public bool SaveContact(Contact contact)
        {
            var ctx = HttpContext.Current;

            if (ctx != null)
            {
                try
                {
                    //need to check records if exist edit!!
                    var currentData = ((List<Contact>)ctx.Cache[CacheKey]).ToList();

                    foreach (Contact user in currentData)
                    {
                        if (user.Id.Equals(contact.Id))
                        {
                            ctx.Cache[CacheKey] = UpdateDB(contact);
                            return true;
                        }
                    }

                    //currentData.Add(contact);
                    ctx.Cache[CacheKey] = InsertDB(contact);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }

            return false;
        }

        
    }
}