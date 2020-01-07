using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebREST.Models;
using WebREST.Services;

namespace WebREST.Controllers
{
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
                string parse = result[1].Replace("DOT", ".");
                var product = contactRepository.GetAllContacts().FirstOrDefault((p) => p.Email == parse);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            else if (id.ToLower().IndexOf("!") >= 0)
            {
                string[] result = new string[2];
                result = id.Split('!');
                var product = contactRepository.GetAllContacts().FirstOrDefault((p) => p.Id == Convert.ToInt32(result[1]));
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            else if (id.ToLower().IndexOf("*") >= 0)
            {
                string[] result = new string[2];
                result = id.Split('!');
                string[] result2 = new string[2];
                result2 = result[1].Split('~');
                string parse = result2[0].Replace("DOT", ".");
                var product = contactRepository.LoginCheck(parse, result2[1]);
                if (!product)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            else
            {
                string parse = id.Replace("DOT", ".");
                var product = contactRepository.DeleteContacts(parse);
                if (!product)
                {
                    return NotFound();
                }
                return Ok(product);
            }
        }

    }

}
