using Employee_Management_Services.GlobalObjects;
using DataObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Employee_Management_Services.Controllers
{
    public class EmployeeController : Controller
    {

        #region Data Members

        log4net.ILog _Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      
        public static Employee _Employee = null;

        #endregion

        #region Public Methods

        /// <summary>
        /// Consumes the API that returns the details of a particular employee
        /// </summary>
        /// <param name="id">Employee ID of the employee whose details you want to see</param>
        /// <param name="userdetail">Employee ID of the user who logged in.(For authentication)</param>
        /// <returns></returns>
        public ActionResult Details(int id, string userdetail,string organisationID)
        {
            _Logger.Info("Requesting details of employee with Employee ID " + id.ToString());
            if (Session["Security"] != null)
                if (userdetail != Session["Security"].ToString() || organisationID!=Session["Organisation"].ToString())
                    return RedirectToAction("ERROR401", "Home");
            if (Session["AdminSecurity"] != null || organisationID != Session["Organisation"].ToString())
                return RedirectToAction("Index", "Admin", new { id = Session["AdminSecurity"].ToString(), organisationID = Session["Organisation"].ToString() });
            try
            {               
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                    var ResponeTask = Client.GetAsync(GlobalHelpers.GetEmployeeDetails + id + "&orgID=" + organisationID);
                    ResponeTask.Wait();
                    var Result = ResponeTask.Result;
                    if (Result.IsSuccessStatusCode)
                    {
                        _Logger.Info("Details delivered.");
                        var ReadTask = Result.Content.ReadAsAsync<Employee>();
                        ReadTask.Wait();
                        _Employee = ReadTask.Result;
                    }
                }
                return View(_Employee);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }
        /// <summary>
        /// Consumes the API that returns the details of the employee that is logged in to the application
        /// </summary>
        /// <param name="id">Employee ID of the employee who has logged into the application</param>
        /// <returns></returns>
        public ActionResult UserAccountInfo(int id , string organisationID)
        {
            if (Session["Security"] != null)
                if (id.ToString() != Session["Security"].ToString() || organisationID != Session["Organisation"].ToString())
                    return RedirectToAction("ERROR401", "Home");
            if (Session["AdminSecurity"] != null || organisationID != Session["Organisation"].ToString())
                return RedirectToAction("ERROR401", "Home");
            try
            {
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                    var ResponeTask = Client.GetAsync(GlobalHelpers.GetEmployeeDetails + id + "&orgID=" + organisationID);
                    ResponeTask.Wait();
                    var Result = ResponeTask.Result;
                    if (Result.IsSuccessStatusCode)
                    {
                        var ReadTask = Result.Content.ReadAsAsync<Employee>();
                        ReadTask.Wait();
                        _Employee = ReadTask.Result;
                    }
                }
                return View(_Employee);
            }
            catch (MySqlException ex)
            {
                _Logger.Error(ex.ToString());
                return RedirectToAction("DatabaseError", "Home");
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }
        /// <summary>
        /// SignOut
        /// </summary>
        /// <returns>redirects to the login page</returns>
        public ActionResult SignOut()
        {
            Session.Abandon();
            try
            {
                _Logger.Info("SignOut Process Completed");
                FormsAuthentication.SignOut();
                FormsAuthentication.RedirectToLoginPage("Home/Login");
                return RedirectToAction("Home/Login");
            }

            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        /// <summary>
        /// Consumes the API that returns the main grid with the list of employees based on certain parameters such as Sort Order, Search String, Department Filter and Gender Filter
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <param name="departmentFilter"></param>
        /// <param name="genderFilter"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult Index(string id,string organisationID, string sortOrder, string currentFilter, string searchString, int? page, string departmentFilter, string genderFilter)
        {
            try
            {
                if (Session["Security"] != null)
                    if (id != Session["Security"].ToString() || organisationID != Session["Organisation"].ToString())
                        return RedirectToAction("ERROR401", "Home");
                if (Session["AdminSecurity"] != null || organisationID != Session["Organisation"].ToString())
                    return RedirectToAction("Index", "Admin", new { id = Session["AdminSecurity"].ToString(), organisationID = Session["Organisation"].ToString() });
                SetFiltersForMainGrid(ref sortOrder, ref searchString, ref departmentFilter, ref genderFilter);
                int pageNumber;
                int lowerLimit;
                EmployeePagedModel model;
                SetPaginationParameters(page, out pageNumber, out lowerLimit, out model);
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                    EmployeeParameters Parameters = new EmployeeParameters(organisationID,sortOrder, searchString, departmentFilter, genderFilter, lowerLimit, 10);
                    var Result = Client.PostAsJsonAsync(GlobalHelpers.GetAllActiveEmployees, Parameters).Result;
                    if (Result.IsSuccessStatusCode)
                    {
                        ProcessEmployeeDataForMainGrid(model, Result);
                        _Logger.Info("Employee list loaded.");
                        return View(model);
                    }
                }                
                return RedirectToAction("ERROR404", "Home");
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        /// <summary>
        /// Consumes the API that returns the details of the employee so that we can edit them
        /// </summary>
        /// <param name="id">Employee ID of the employee whose details you want to edit</param>
        /// <param name="user"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult Edit(int id, string user,string organisationID)
        {
            if (Session["Security"] != null)
                if (id != int.Parse(Session["Security"].ToString()) || user != Session["Security"].ToString() || organisationID != Session["Organisation"].ToString())
                    return RedirectToAction("ERROR401", "Home");
            if (Session["AdminSecurity"] != null || organisationID != Session["Organisation"].ToString())
                return RedirectToAction("Index", "Admin", new { id = Session["AdminSecurity"].ToString(), organisationID= Session["Organisation"].ToString() });
            try
            {
                if (ModelState.IsValid)
                {
                    using (var Client = new HttpClient())
                    {                       
                        Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                        Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                        var ResponeTask = Client.GetAsync(GlobalHelpers.GetEmployeeDetails + id + "&orgID=" + organisationID);
                        ResponeTask.Wait();
                        var Result = ResponeTask.Result;
                        if (Result.IsSuccessStatusCode)
                        {
                            _Logger.Info("Loading details of employee ID " + id.ToString() + " initiated.");
                            var readTask = Result.Content.ReadAsAsync<Employee>();
                            readTask.Wait();
                            _Employee = readTask.Result;
                        }
                    }
                }
                return View(_Employee);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        /// <summary>
        /// Consumes the API that updates the details of the employee
        /// </summary>
        /// <param name="id">Employee ID of the employee whose details we want to edit</param>
        /// <param name="employee">Contains updated employee details</param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Employee employee, string user, string organisationID)
        {
            _Logger.Info("Editing employee  " + JsonConvert.SerializeObject(employee).ToString() + " initiated.");
            try
            {
                employee.Employee_Id = id;
                employee.OrganisationId = organisationID;
                ModelState.Remove("password");
                ModelState.Remove("Status");
                ModelState.Remove("EndDate");
                if (ModelState.IsValid)
                {
                    using (var Client = new HttpClient())
                    {                        
                        Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                        Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                        var Response = Client.PostAsJsonAsync(GlobalHelpers.EditEmployee, employee).Result;
                        if (Response.IsSuccessStatusCode)
                        {
                            _Logger.Info("Editing employee with Employee ID= " + id.ToString() + " completed.");
                            return RedirectToAction("Index", "Employee", new { id = Session["Security"].ToString(), organisationID = Session["Organisation"]});
                        }
                        else if (Response.StatusCode.ToString() == "422")
                        {
                            _Logger.Info("Editing employee with Employee ID= " + id.ToString() + " failed.");
                            ModelState.AddModelError("Email", "Email already exists");
                            return View(employee);
                        }
                        else RedirectToAction("ERROR404", "Home");
                    }
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        /// <summary>
        /// To change employee password
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword(int id, string organisationID, string user)
        {
            try
            {
                if (Session["Security"] == null && Session["AdminSecurity"] == null)
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Home");
                }
                if (Session["Security"] != null)
                    if (id != int.Parse(Session["Security"].ToString()) || user != Session["Security"].ToString() || organisationID != Session["Organisation"].ToString())
                        return RedirectToAction("ERROR401", "Home");
                if (Session["AdminSecurity"] != null || organisationID != Session["Organisation"].ToString())
                    return RedirectToAction("Index", "Admin", new { id = Session["AdminSecurity"].ToString(), organisationID = Session["Organisation"].ToString() });
                return View();
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        /// <summary>
        /// Consumes the API that changes the employee password
        /// </summary>
        /// <param name="employee">contains old and new password for verification and updation</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ChangePassword(Employee employee)
        {
            _Logger.Info("Request for changing the account password for employee initiated.");
            try
            {
                if (employee.Password != employee.ConfirmPassword)
                    return View();
                using (var Client = new HttpClient())
                {                    
                    employee.Employee_Id = int.Parse(Session["Security"].ToString());
                    employee.OrganisationId = Session["Organisation"].ToString();
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                    var Response = Client.PostAsJsonAsync(GlobalHelpers.ChangeEmployeePassword, employee).Result;
                    if (Response.IsSuccessStatusCode)
                    {
                        _Logger.Info("Password Changed Successfully.");
                        return RedirectToAction("SignOut");
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// To set the values of the filters for main grid page
        /// </summary>
        /// <param name="sortOrder">Sort Order</param>
        /// <param name="searchString">Search string</param>
        /// <param name="departmentFilter">Department Filter</param>
        /// <param name="genderFilter">Gender Filter</param>
        private void SetFiltersForMainGrid(ref string sortOrder, ref string searchString, ref string departmentFilter, ref string genderFilter)
        {
            try
            {
                if (departmentFilter == null)
                    departmentFilter = "";
                if (genderFilter == null)
                    genderFilter = "";
                if (sortOrder == null)
                    sortOrder = "";
                TempData["genderfilter"] = genderFilter;
                TempData["departmentfilter"] = departmentFilter;
                TempData["searchstring"] = searchString;
                SetSortOrderForMainGrid(ref sortOrder);
                ViewBag.genderfilter = genderFilter;
                ViewBag.departmentfilter = departmentFilter;
                ViewBag.CurrentFilter = searchString;
                if (searchString == null && sortOrder == "" && departmentFilter == "" && genderFilter == "")
                    ViewBag.viewall = "no";
                else
                    ViewBag.viewall = "yes";
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Sets sort order for the main grid page
        /// </summary>
        /// <param name="sortOrder">Sort Order</param>
        private void SetSortOrderForMainGrid(ref string sortOrder)
        {
            try
            {
                TempData["sort"] = sortOrder;
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm0 = sortOrder == "eid_desc" ? "eid" : "eid_desc";
                ViewBag.NameSortParm1 = sortOrder == "name" ? "name_desc" : "name";
                ViewBag.NameSortParm2 = sortOrder == "gender" ? "gender_desc" : "gender";
                ViewBag.NameSortParm3 = sortOrder == "dept" ? "dept_desc" : "dept";
                ViewBag.NameSortParm4 = sortOrder == "email" ? "email_desc" : "email";
                ViewBag.NameSortParm5 = sortOrder == "contact" ? "contact_desc" : "contact";
                ViewBag.DateSortParm1 = sortOrder == "Date" ? "date_desc" : "Date";
                ViewBag.DateSortParm2 = sortOrder == "JoinDate" ? "join_date_desc" : "JoinDate";
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }

        }

        /// <summary>
        /// Sets pagination parameters for the main grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageNumber"></param>
        /// <param name="lowerLimit"></param>
        /// <param name="model"></param>
        private void SetPaginationParameters(int? page, out int pageNumber, out int lowerLimit, out EmployeePagedModel model)
        {
            try
            {
                if (page == null)
                {
                    pageNumber = 1;
                }
                else pageNumber = page.Value;
                lowerLimit = (pageNumber - 1) * 10;
                TempData["CurrentPage"] = pageNumber;
                model = new EmployeePagedModel
                {
                    Employees = new List<Employee>(),
                    PageCount = 0,
                    PageNumber = 1
                };
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Gets employee list as per all the filters,sortorder and searchstring
        /// </summary>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private void ProcessEmployeeDataForMainGrid(EmployeePagedModel model, HttpResponseMessage result)
        {
            try
            {
                var ReadTask = result.Content.ReadAsAsync<EmployeePagedModel>();
                ReadTask.Wait();
                model.Employees = ReadTask.Result.Employees;
                model.PageNumber = ReadTask.Result.PageNumber;
                float PaginationNumber;
                if ((ReadTask.Result.PageCount % ReadTask.Result.PageNumber) == 0)
                {
                    PaginationNumber = (ReadTask.Result.PageCount / ReadTask.Result.PageNumber);
                    model.PageCount = int.Parse(Math.Ceiling(PaginationNumber).ToString());
                }
                else
                {
                    PaginationNumber = (ReadTask.Result.PageCount / ReadTask.Result.PageNumber) + 1;
                    model.PageCount = int.Parse(Math.Ceiling(PaginationNumber).ToString());
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        #endregion

    }
}