using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.ClientServices.Providers;
using System.Web.Mvc;
using SecurityEssentialsDemo.Models;

namespace SecurityEssentialsDemo.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult Index() => View();
    }
}