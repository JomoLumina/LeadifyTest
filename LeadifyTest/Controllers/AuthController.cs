using LeadifyTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using LeadifyTest.CustomLibraries;
using System.Data.SqlClient;
using System.Data.Entity.Validation;

namespace LeadifyTest.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        // GET: Auth
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users model)
        {
            if (!ModelState.IsValid) //Checks if input fields have the correct format
            {
                return View(model); //Returns the view with the input values so that the user doesn't have to retype again
            }

            using (var db = new MainDbContext())
            {
                var emailCheck = db.Users.FirstOrDefault(u => u.Email == model.Email);

                if (!String.IsNullOrEmpty(emailCheck.Email))
                {
                    var getUsername = db.Users.Where(u => u.Email == model.Email).Select(u => u.Username);
                    var materializeName = getUsername.ToList();
                    var name = materializeName[0];

                    var getPassword = db.Users.Where(u => u.Email == model.Email).Select(u => u.Password);
                    var materializePassword = getPassword.ToList();
                    var password = materializePassword[0];
                    var decryptedPassword = CustomDecrypt.Decrypt(password);

                    if (model.Email != null && model.Password == decryptedPassword)
                    {
                        var getEmail = db.Users.Where(u => u.Email == model.Email).Select(u => u.Email);
                        var materializeEmail = getEmail.ToList();
                        var email = materializeEmail[0];

                        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.Name, name) }, "ApplicationCookie");

                        var ctx = Request.GetOwinContext();
                        var authManager = ctx.Authentication;

                        authManager.SignIn(identity);

                        return RedirectToAction("Index", "Home");
                    }

                }
            }

            Warning("Invalid Email Address or Password", true);
            return View(model);
        }

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(Users model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new MainDbContext())
                {
                    var queryUser = db.Users.FirstOrDefault(u => u.Email == model.Email);
                    if (queryUser == null)
                    {
                        var encryptedPassword = CustomEnrypt.Encrypt(model.Password);
                        var user = db.Users.Create();
                        user.Email = model.Email;
                        user.Password = encryptedPassword;
                        user.Username = !String.IsNullOrEmpty(model.Username) ? model.Username : "User";

                        db.Users.Add(user);
                        db.SaveChanges();
                        Success(string.Format("<b>{0}</b> was successfully created", user.Username), true);
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        Danger("User with Email already exist. Please check your details and try again.", true);
                        return RedirectToAction("Registration");
                    }
                }
            }
            else
            {
                Danger("Error creating contact. Please check your form and try again.", true);
            }
            return View();
        }
    }
}