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
using PagedList;

namespace LeadifyTest.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var db = new MainDbContext();
            Claim sessionEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email);
            string userEmail = sessionEmail.Value;
            var userIdQuery = db.Users.Where(u => u.Email == userEmail).Select(u => u.Id);
            var userIds = userIdQuery.ToList();

            //pagination
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            ViewBag.ContactIdSortParam = String.IsNullOrEmpty(sortOrder) ? "contact_id_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "first_name" ? "first_name_desc" : "first_name";
            ViewBag.LastNameSortParm = sortOrder == "last_name" ? "last_name_desc" : "last_name";
            ViewBag.EmailSortParm = sortOrder == "email" ? "email_desc" : "email";

            var contacts = db.Contacts.Where(c => c.UserId == userIds.FirstOrDefault());

            if (!String.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(c => c.FirstName.Contains(searchString)
                                       || c.LastName.Contains(searchString) || c.Cellphone.Contains(searchString) || c.Email.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "contact_id_desc":
                    contacts = contacts.OrderByDescending(c => c.ContactId);
                    break;
                case "first_name":
                    contacts = contacts.OrderBy(c => c.FirstName);
                    break;
                case "first_name_desc":
                    contacts = contacts.OrderByDescending(c => c.FirstName);
                    break;
                case "last_name":
                    contacts = contacts.OrderBy(c => c.LastName);
                    break;
                case "last_name_desc":
                    contacts = contacts.OrderByDescending(c => c.LastName);
                    break;
                case "email":
                    contacts = contacts.OrderBy(c => c.Email);
                    break;
                case "email_desc":
                    contacts = contacts.OrderByDescending(c => c.Email);
                    break;
                default:
                    contacts = contacts.OrderBy(c => c.ContactId);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(contacts.ToPagedList(pageNumber, pageSize));
        }


        //public ActionResult Index(string sortOrder)
        //{
        //    ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        //    ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
        //    var students = from s in db.Students
        //                   select s;
        //    switch (sortOrder)
        //    {
        //        case "name_desc":
        //            students = students.OrderByDescending(s => s.LastName);
        //            break;
        //        case "Date":
        //            students = students.OrderBy(s => s.EnrollmentDate);
        //            break;
        //        case "date_desc":
        //            students = students.OrderByDescending(s => s.EnrollmentDate);
        //            break;
        //        default:
        //            students = students.OrderBy(s => s.LastName);
        //            break;
        //    }
        //    return View(students.ToList());
        //}

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