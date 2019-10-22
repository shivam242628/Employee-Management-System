using Employee_Management_Services.GlobalObjects;
using DataObjects;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Security;
using MySql.Data.MySqlClient;

namespace Employee_Management_Services.Controllers
{

    public class HomeController : Controller
    {
        #region Data Members

        log4net.ILog _Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion        

        #region Public Methods

        /// <summary>
        /// Home page of the application. Simply redirects to the login page.
        /// </summary>
        /// <returns></returns>
        [Authorize]       
        public ActionResult Index()
        {
            if (Session["Security"] != null)
            {
                return RedirectToAction("Index", "Employee", new { id = Session["Security"].ToString(), organisationID = Session["Organisation"].ToString() });
            }

            if (Session["AdminSecurity"] != null)
            {
                return RedirectToAction("Index", "Admin", new { userName = Session["AdminSecurity"].ToString(), organisationID = Session["Organisation"].ToString() });
            }
            try
            {
                return RedirectToAction("Login");
            }

            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }


        /// <summary>
        /// Login page of the application
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult Login()
        {
            if (Session["Security"] != null)
            {
                return RedirectToAction("Index", "Employee", new { id = Session["Security"].ToString(), organisationID = Session["Organisation"].ToString() });
            }

            if (Session["AdminSecurity"] != null)
            {
                return RedirectToAction("Index", "Admin", new { userName = Session["AdminSecurity"].ToString(), organisationID = Session["Organisation"].ToString() });
            }
            try
            {               
                FormsAuthentication.SignOut();
                return View();

            }

            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }


        /// <summary>
        /// Consumes the API that validates the credentials of the employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult Login(Employee employee)
        {
            _Logger.Info("An Employee tried to Log in With ID " + employee.Employee_Id.ToString());
            try
            {                
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                    var Response = Client.PostAsJsonAsync(GlobalHelpers.ValidateEmployee, employee).Result;
                    if (Response.IsSuccessStatusCode)
                    {
                        _Logger.Info("Login Successful");
                        FormsAuthentication.RedirectFromLoginPage(employee.Employee_Id.ToString(), false);
                        Session["Security"] = employee.Employee_Id;
                        Session["Organisation"] = employee.OrganisationId;
                        return RedirectToAction("Index", "Employee", new { id = employee.Employee_Id, organisationID = employee.OrganisationId});
                    }
                    else
                    {
                        _Logger.Info("Login Failed");
                        FormsAuthentication.SignOut();
                        return View();
                    }
                }
            }
            catch (MySqlException ex)
            {
                _Logger.Error("Login Failed" + ex.ToString());
                return RedirectToAction("DatabaseError", "Home");
            }
            catch (Exception ex)
            {
                _Logger.Fatal("Login Failed" + ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }


        /// <summary>
        /// 401 Error Page
        /// </summary>
        /// <returns></returns>
        public ActionResult ERROR401()
        {
            try
            {
                return View();
            }

            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");

            }

        }


        /// <summary>
        /// 404 Error Page
        /// </summary>
        /// <returns></returns>
        public ActionResult ERROR404()
        {
            try
            {
                return View();
            }

            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }


        /// <summary>
        /// Database error page
        /// </summary>
        /// <returns></returns>
        public ActionResult DatabaseError()
        {
            try
            {
                return View();
            }

            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        #endregion
    }
}
