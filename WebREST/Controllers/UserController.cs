using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using WebREST.Models;
using WebREST.Services;

namespace WebREST.Controllers
{
    //[EnableCors(origins: "http://localhost:53968", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private ContactRepository contactRepository;

        public UserController()
        {
            this.contactRepository = new ContactRepository();
        }

        //public Contact[] Get()
        //{
        //    return contactRepository.GetAllContacts();
        //}

        public List<Contact> Get()
        {
            return contactRepository.GetAllContacts();
        }

        public HttpResponseMessage Post(Contact contact)
        {
            this.contactRepository.SaveContact(contact);

            var response = Request.CreateResponse<Contact>(System.Net.HttpStatusCode.Created, contact);

            return response;
        }

        //public IHttpActionResult GetUser(int id)
        //{
        //    var product = contactRepository.GetAllContacts().FirstOrDefault((p) => p.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(product);
        //}

        public IHttpActionResult GetUser(string id)
        {
            if (id.ToLower().IndexOf("|") >= 0)
            {
                string[] result = new string[2];
                result = id.Split('|');
                if (result[0].Equals("edit"))
                {
                    string parse = result[1].Replace("DOT", ".");
                    var product = contactRepository.GetAllContacts().FirstOrDefault((p) => p.Email == parse);
                    if (product == null)
                    {
                        return NotFound();
                    }
                    return Ok(product);
                }
                else if (result[0].Equals("find"))
                {
                    string parse = "";
                    if (result[1].IndexOf("DOT") >= 0)
                    {
                        parse = result[1].Replace("DOT", ".");
                    }
                    else
                    {
                        parse = result[1];
                    }
                    
                    var product = contactRepository.GetAllContacts().FirstOrDefault((p) => p.Email == parse);
                    if (product == null)
                    {
                        return NotFound();
                    }
                    return Ok(product);
                }
                else if (result[0].Equals("delete"))
                {
                    string parse = "";
                    if (result[1].IndexOf("DOT") >= 0)
                    {
                        parse = result[1].Replace("DOT", ".");
                    }
                    else
                    {
                        parse = result[1];
                    }
                    var product = contactRepository.DeleteContacts(parse);
                    if (!product)
                    {
                        return NotFound();
                    }
                    return Ok(product);
                }
                else if (result[0].Equals("login"))
                {
                    string[] login = new string[2];
                    login = result[1].Split('^');
                    string email = "";
                    if (login[0].IndexOf("DOT") >= 0)
                    {
                        email = login[0].Replace("DOT", ".");
                    }
                    else
                    {
                        email = login[0];
                    }
                    
                    string password = login[1];
                    var product = contactRepository.LoginCheck(email, password);
                    if (!product)
                    {
                        return NotFound();
                    }
                    return Ok(product);
                }

                return Ok();
            }
            //else if (id.ToLower().IndexOf("*") >= 0)
            else
            {
                return NotFound();
            }
        }

    }

}
