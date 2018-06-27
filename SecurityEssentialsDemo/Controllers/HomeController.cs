using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.ClientServices.Providers;
using System.Web.Mvc;
using Ganss.XSS;
using SecurityEssentialsDemo.Models;

namespace SecurityEssentialsDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        #region SQL Injection
        [HttpGet]
        public ActionResult SQLInjection()
        {
            ViewBag.Message = "SqlInjection Demo";
            using (var ctx = new SampleModel())
            {

                return View(ctx.InjectionSampleItems.AsNoTracking().ToArray());
            }
        }

        [HttpPost]
        public ActionResult SQLInjection(string text)
        {
            using (var ctx = new SampleModel())
            {
                var newItem = new InjectionSampleItem()
                {
                    Id = Guid.NewGuid(),
                    Text = text
                };
                ctx.InjectionSampleItems.Add(newItem);
                ctx.SaveChanges();
            }

            return RedirectToAction(nameof(SQLInjection));
        }

        [HttpGet]
        public ActionResult SQLInjectionSearch(string input)
        {
            using (var ctx = new SampleModel())
            {
                var items = ctx.Database.SqlQuery<InjectionSampleItem>(
                    sql: $"SELECT [Id], [Text] FROM [dbo].[InjectionSampleItems] WHERE [Text] LIKE '%{input}%'")
                    .ToArray();

                return View(nameof(SQLInjection), items);
            }
        }

        [HttpGet]
        public ActionResult SqlInjectionSafeSearch(string safeinput)
        {
            using (var ctx = new SampleModel())
            {
                var parameter = new SqlParameter(parameterName: "safeInput", value: safeinput);

                var items = ctx.Database.SqlQuery<InjectionSampleItem>(
                        sql: $"SELECT [Id], [Text] FROM [dbo].[InjectionSampleItems] WHERE [Text] LIKE '%' + @safeInput + '%'",
                        parameters: parameter)
                    .ToArray();

                return View(nameof(SQLInjection), items);
            }
        }

        #endregion

        #region Broken Authentication
        private static readonly Credentials[] credentials = new Credentials[]
        {
            new Credentials("rafal", "1234"),
            new Credentials("user", "password"),
            new Credentials("admin", "admin"),
        };

        [HttpGet]
        public ActionResult BrokenAuth(string msg = "") => View(model: msg);

        [HttpPost]
        public ActionResult BrokenAuth(Credentials inputCreds)
        {
            var foundUser = credentials.FirstOrDefault(c => c.UserName == inputCreds.UserName);
            if (foundUser == null)
                return RedirectToAction(nameof(BrokenAuth), new { msg = $"No user with name {inputCreds.UserName}" });

            if (foundUser.Password != inputCreds.Password)
            {
                return RedirectToAction(nameof(BrokenAuth), new { msg = "Wrong password" });
            }

            return RedirectToAction(nameof(BrokenAuth), new { msg = "Logged in!" });
        }
        #endregion

        #region XSS
        [HttpGet]
        public ActionResult XSS()
        {
            using (var ctx = new SampleModel())
            {
                var items = ctx.XSSSampleItems.AsNoTracking().ToArray();

                return View(items);
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult XSS(string text)
        {
            using (var ctx = new SampleModel())
            {
                ctx.XSSSampleItems.Add(new XSSSampleItem()
                {
                    Id = Guid.NewGuid(),
                    Text = text
                });

                ctx.SaveChanges();

                return RedirectToAction(nameof(XSS));
            }
        }

        
        [HttpPost]
        public ActionResult XSSValidate(string text)
        {
            using (var ctx = new SampleModel())
            {
                ctx.XSSSampleItems.Add(new XSSSampleItem()
                {
                    Id = Guid.NewGuid(),
                    Text = text
                });

                ctx.SaveChanges();

                return RedirectToAction(nameof(XSS));
            }
        }

        //Using HtmlSanitizer nuget
        [HttpPost, ValidateInput(false)]
        public ActionResult XSSSanitize(string text)
        {
            using (var ctx = new SampleModel())
            {
                var sanitizer = new HtmlSanitizer();
                var sanitizedInput = sanitizer.Sanitize(text);

                ctx.XSSSampleItems.Add(new XSSSampleItem()
                {
                    Id = Guid.NewGuid(),
                    Text = sanitizedInput
                });

                ctx.SaveChanges();

                return RedirectToAction(nameof(XSS));
            }
        }

        [HttpGet]
        public ActionResult ResetXSS()
        {
            using (var ctx = new SampleModel())
            {
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE [dbo].[XSSSampleItems]");

                return RedirectToAction(nameof(XSS));
            }
        }

        [HttpGet, ValidateInput(false)]
        public ActionResult ReflectedXSS(string text = "")
        {
            HttpContext.Response.AddHeader("X-XSS-Protection", "0");
            return View(model: text);
        }
        #endregion
    }

    public class Credentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public Credentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public Credentials()
        {
        }
    }
}