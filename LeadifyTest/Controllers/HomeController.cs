using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadifyTest.Models;
using System.Security.Claims;
using System.Threading;
using System.Data.Entity;
using System.Data.SqlClient;

namespace LeadifyTest.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            var db = new MainDbContext();
            Claim sessionEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email);
            string userEmail = sessionEmail.Value;
            var userIdQuery = db.Users.Where(u => u.Email == userEmail).Select(u => u.Id);
            var userIds = userIdQuery.ToList();

            return View(db.Contacts.Where(c => c.UserId == userIds.FirstOrDefault()).ToList());
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Contacts contact)
        {
            if (ModelState.IsValid)
            {

                using (var db = new MainDbContext())
                {
                    Claim sessionEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email);
                    string userEmail = sessionEmail.Value;
                    var userIdQuery = db.Users.Where(u => u.Email == userEmail).Select(u => u.Id);
                    var userIds = userIdQuery.ToList();

                    string fisrtName = Request.Form["FirstName"];
                    string lastName = Request.Form["LastName"];
                    string cellphone = Request.Form["Cellphone"];
                    string email = Request.Form["Email"];

                    var dbList = db.Contacts.Create();

                    dbList.FirstName = fisrtName;
                    dbList.LastName = lastName;
                    dbList.Cellphone = cellphone;
                    dbList.Email = email;
                    dbList.UserId = userIds.FirstOrDefault();

                    db.Contacts.Add(dbList);
                    db.SaveChanges();
                    Success(string.Format("<b>{0}</b> was successfully created", dbList.FirstName), true);
                    return RedirectToAction("Index");
                }
            }
            else
            {

                Danger("Error creating contact. Please check your form and try again.", true);
                return View();
            }

           
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var db = new MainDbContext();
            var model = new Contacts();
            model = db.Contacts.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Contacts list)
        {
            var db = new MainDbContext();
            string fisrtName = Request.Form["FirstName"];
            string lastName = Request.Form["lastName"];
            string cellphone = Request.Form["cellphone"];
            string email = Request.Form["email"];
            int contactId = Int32.Parse(Request.Form["contactId"]);

            if (ModelState.IsValid)
            {
                Claim sessionEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email);
                string userEmail = sessionEmail.Value;
                var userIdQuery = db.Users.Where(u => u.Email == userEmail).Select(u => u.Id);
                var userIds = userIdQuery.ToList();

                list.ContactId = contactId;
                list.FirstName = fisrtName;
                list.LastName = lastName;
                list.Cellphone = cellphone;
                list.Email = email;
                list.UserId = userIds.FirstOrDefault();

                db.Entry(list).State = EntityState.Modified;
                db.SaveChanges();
                Success(string.Format("<b>{0}</b> was successfully updated", list.FirstName), true);

                return RedirectToAction("Index");

            }
            else
            {
                Danger("Error updating contact. Please check your form and try again.", true);
                var model = new Contacts();
                model = db.Contacts.Find(contactId);
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var db = new MainDbContext();
            var model = db.Contacts.Find(id);

            if (model == null)
            {
                Danger("Error deleting Contact. Please try again.", true);
                return RedirectToAction("Index");
            }

            db.Contacts.Remove(model);
            db.SaveChanges();
            Success(string.Format("<b>{0}</b> was successfully removed", model.FirstName), true);
            return RedirectToAction("Index");
        }
    }
}