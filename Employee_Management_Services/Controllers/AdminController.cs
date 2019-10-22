using Employee_Management_Services.GlobalObjects;
using DataObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Employee_Management_Services.Controllers
{
    public class AdminController : Controller
    {
        #region Data Members
        log4net.ILog _Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Employee _Employee = null;
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets the filters for the main grid
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="searchString"></param>
        /// <param name="departmentFilter"></param>
        /// <param name="genderFilter"></param>
        private void SetFiltersForMainGrid(ref string sortOrder, ref string searchString, ref string departmentFilter, ref string genderFilter)
        {
            try
            {
                if (sortOrder == null)
                    sortOrder = "";
                if (departmentFilter == null)
                    departmentFilter = "";
                if (genderFilter == null)
                    genderFilter = "";
                if (searchString == null && sortOrder == "" && departmentFilter == "" && genderFilter == "")
                    ViewBag.viewall = "no";
                else ViewBag.viewall = "yes";
                SetSortOrderForMainGrid(ref sortOrder, ref searchString, ref departmentFilter, ref genderFilter);
                TempData["sorting"] = sortOrder;
                TempData["genderfilter"] = genderFilter;
                TempData["departmentfilter"] = departmentFilter;
                TempData["searchstring"] = searchString;
                ViewBag.orgID = Session["Organisation"];
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// Sets the sortorder for the main grid
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="searchString"></param>
        /// <param name="departmentFilter"></param>
        /// <param name="genderFilter"></param>
        private void SetSortOrderForMainGrid(ref string sortOrder, ref string searchString, ref string departmentFilter, ref string genderFilter)
        {
            try
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm0 = sortOrder == "eid" ? "eid_desc" : "eid";
                ViewBag.NameSortParm1 = sortOrder == "name" ? "name_desc" : "name";
                ViewBag.NameSortParm2 = sortOrder == "gender" ? "gender_desc" : "gender";
                ViewBag.NameSortParm3 = sortOrder == "dept" ? "dept_desc" : "dept";
                ViewBag.NameSortParm4 = sortOrder == "email" ? "email_desc" : "email";
                ViewBag.NameSortParm5 = sortOrder == "contact" ? "contact_desc" : "contact";
                ViewBag.DateSortParm1 = sortOrder == "Date" ? "date_desc" : "Date";
                ViewBag.DateSortParm2 = sortOrder == "JoinDate" ? "join_date_desc" : "JoinDate";
                ViewBag.NameSortParm6 = sortOrder == "orgid" ? "orgid_desc" : "orgid";
                ViewBag.genderfilter = genderFilter;
                ViewBag.departmentfilter = departmentFilter;
                ViewBag.CurrentFilter = searchString;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// Sets the pagination parameters for the main grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="lowerLimit"></param>
        /// <param name="model"></param>
        private void SetPaginationParameters(int? page, out int lowerLimit, out EmployeePagedModel model)
        {
            try
            {
                int pagenumber;
                if (page == null)
                {
                    pagenumber = 1;
                }
                else pagenumber = page.Value;
                lowerLimit = (pagenumber - 1) * 10;
                TempData["CurrentPage"] = pagenumber;
                model = new EmployeePagedModel
                {
                    Employees = new List<Employee>(),
                    PageCount = 0,
                    PageNumber = 1
                };
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// Set some more pagination parameters
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
                _Logger.Error(ex.ToString());
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new Employee
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult Create(string userName)
        {
            if (Session["Security"] != null)
            {
                return RedirectToAction("ERROR401", "Home");
            }
            if (Session["AdminSecurity"] == null || userName != Session["AdminSecurity"].ToString())
            {
                return RedirectToAction("ERROR401", "Home");
            }
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
        /// Consumes the API that creates a new employee
        /// </summary>
        /// <param name="employee">contains all the details of the employee</param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Employee employee, string username)
        {
            _Logger.Info("Adding new employee." + JsonConvert.SerializeObject(employee).ToString());            
            try
            {
                employee.OrganisationId = Session["Organisation"].ToString();
                ModelState.Remove("Status");
                ModelState.Remove("EndDate");
                ModelState.Remove("OrganisationId");
                if (ModelState.IsValid)
                {
                    using (var Client = new HttpClient())
                    {                        
                        Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                        Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                        var Response = Client.PostAsJsonAsync(GlobalHelpers.CreateEmployee, employee).Result;
                        if (Response.IsSuccessStatusCode)
                        {
                            _Logger.Info("New Employee Added with email ID: " + employee.Email);
                            var ReadTask = Response.Content.ReadAsAsync<Employee>();
                            ReadTask.Wait();
                            _Employee = ReadTask.Result;                            
                            return RedirectToAction("Index", "Admin", new { userName = Session["AdminSecurity"].ToString() , organisationID=Session["Organisation"] });
                        }
                        else if(Response.StatusCode.ToString() == "422")
                        {
                            _Logger.Info("Adding employee with Employee email ID= " + employee.Email + " failed.");
                            ModelState.AddModelError("Email", "Email already exists");
                            return View(employee);
                        }
                        else if (Response.StatusCode.ToString() == "555")
                        {
                            _Logger.Info("Adding employee with Employee email ID= " + employee.Email + " failed.");
                            ModelState.AddModelError("OrganisationId", "Organisation ID doesn't exists");
                            return View(employee);
                        }
                        else return RedirectToAction("ERROR404", "Home");
                    }
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                _Logger.Fatal("Employee not added." + ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        /// <summary>
        /// Consumes the API that returns the details of the employee we want to edit
        /// </summary>
        /// <param name="id">Employee ID of the employee to be edited</param>
        /// <param name="username"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult Edit(int id, string userName,string organisationID)
        {
            if (Session["Security"] != null)
                return RedirectToAction("ERROR401", "Home");
            if (Session["AdminSecurity"] == null || userName != Session["AdminSecurity"].ToString())
            {
                return RedirectToAction("ERROR401", "Home");
            }
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
        /// Consumes the API that edits the details of the employee
        /// </summary>
        /// <param name="id">Employee ID of the employee to be edited</param>
        /// <param name="employee">contains updated details of that employee</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Employee employee, string organisationID)
        {
            _Logger.Info("Edit Request for Employee  " + JsonConvert.SerializeObject(employee).ToString() + " initiated.");
            try
            {
                employee.Employee_Id = id;
                ModelState.Remove("password");
                ModelState.Remove("Status");
                ModelState.Remove("EndDate");
                if (ModelState.IsValid)
                {
                    using (var Client = new HttpClient())
                    {                        
                        Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                        Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                        var Response = Client.PostAsJsonAsync(GlobalHelpers.EditEmployeeWithAdmin, employee).Result;
                        if (Response.IsSuccessStatusCode)
                        {
                            _Logger.Info("Editing employee with Employee ID= " + id.ToString() + " completed.");
                            return RedirectToAction("Index", new { userName = Session["AdminSecurity"].ToString() ,organisationID = Session["Organisation"] });
                        }
                        else if(Response.StatusCode.ToString() == "422")
                        {
                            _Logger.Info("Editing employee with Employee ID= " + id.ToString() + " completed.");
                            ModelState.AddModelError("Email", "Email already exists");                            
                            return View(employee);
                        }           
                        else return RedirectToAction("ERROR404", "Home");
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
        /// Consumes the API that changes the status of an employee
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        /// <param name="organisationID"></param>
        /// <returns></returns>
        public ActionResult ChangeStatus(int id, string userName, string organisationID)
        {
            if (Session["Security"] != null)
                return RedirectToAction("ERROR401", "Home");
            if (Session["AdminSecurity"] == null || userName != Session["AdminSecurity"].ToString())
            {
                return RedirectToAction("ERROR401", "Home");
            }
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
        /// Consumes the API that changes the status of an employee
        /// </summary>
        /// <param name="id"></param>
        /// <param name="employee"></param>
        /// <param name="organisationID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeStatus(int id, Employee employee, string organisationID)
        {
            employee.Employee_Id = id;
            employee.OrganisationId = organisationID;
            _Logger.Info("Change Status Request for Employee  " + JsonConvert.SerializeObject(employee).ToString() + " initiated.");
            try
            {
                if(employee.Status != "INACTIVE")
                {
                    ModelState.AddModelError("Status", "Status must be INACTIVE to add last date.");
                    return View(employee);
                }
                    using (var Client = new HttpClient())
                    {
                        Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                        Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                        var Response = Client.PostAsJsonAsync(GlobalHelpers.ChangeStatus, employee).Result;
                        if (Response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index", new { userName = Session["AdminSecurity"].ToString(), organisationID = Session["Organisation"] });
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
        /// Admin Login Page
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult Login()
        {
            if (Session["Security"] != null)
            {
                return RedirectToAction("ERROR401", "Home");
            }
            if (Session["AdminSecurity"] != null)
            {
                return RedirectToAction("Index", "Admin", new { userName = Session["AdminSecurity"].ToString(), organisationID = Session["Organisation"].ToString() });
            }
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
        /// Consumes the API that validates the credentials of the Administrator
        /// </summary>
        /// <param name="admin">Contains username and password of admin</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult Login(Admin admin)
        {
            _Logger.Info("Login process for " + admin.Admin_User_Name + " initiated.");
            try
            {
                using (var Client = new HttpClient())
                {                   
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                    var Response = Client.PostAsJsonAsync(GlobalHelpers.ValidateAdmin, admin).Result;
                    if (Response.IsSuccessStatusCode)
                    {
                        _Logger.Info("Login process for " + admin.Admin_User_Name + " succeeded.");
                        FormsAuthentication.RedirectFromLoginPage(admin.Admin_User_Name, false);
                        Session["AdminSecurity"] = admin.Admin_User_Name;
                        if (admin.OrganisationID == "0")
                        {
                            Session["Organisation"] = "0";
                            return RedirectToAction("ViewOrganisations","Admin", new { userName = admin.Admin_User_Name, organisationID = admin.OrganisationID });
                        }
                        else
                        {
                            Session["Organisation"] = admin.OrganisationID;
                            return RedirectToAction("Index", "Admin", new { userName = admin.Admin_User_Name, organisationID = admin.OrganisationID });
                        }
                    }
                    else
                    {
                        _Logger.Info("Login process for " + admin.Admin_User_Name + " failed.");
                        FormsAuthentication.SignOut();
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }        

        /// <summary>
        /// Change admin password
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword(string userName, string organisationID)
        {
            try
            {
                if (Session["Security"] != null)
                {
                    return RedirectToAction("ERROR401", "Home");
                }
                if (Session["AdminSecurity"] == null || userName != Session["AdminSecurity"].ToString() || organisationID != Session["Organisation"].ToString())
                {
                    return RedirectToAction("ERROR401", "Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        /// <summary>
        /// Consumes the API that changes the admin password
        /// </summary>
        /// <param name="admin">contains old and new password of admin to verify and update</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ChangePassword(Admin admin)
        {
            _Logger.Info("Password change process for " + admin.Admin_User_Name + " initiated.");
            try
            {
                admin.Admin_User_Name = Session["AdminSecurity"].ToString();
                admin.OrganisationID = Session["Organisation"].ToString();
                if(admin.Password!=admin.ConfirmPassword)
                    return View();
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                    var Response = Client.PostAsJsonAsync(GlobalHelpers.ChangeAdminPassword, admin).Result;
                    if (Response.IsSuccessStatusCode)
                    {
                        _Logger.Info("Password change process for " + admin.Admin_User_Name + " succeeded.");
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

        /// <summary>
        /// Account Info page of Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UserAccountInfo(string id, string organisationID)
        {
            if (Session["Security"] != null)
            {
                return RedirectToAction("ERROR401", "Home");
            }
            if (Session["AdminSecurity"] == null || id != Session["AdminSecurity"].ToString() || organisationID != Session["Organisation"].ToString())
            {
                return RedirectToAction("ERROR401", "Home");
            }
            ViewBag.OrgID = organisationID;
            try
            {
                Admin admin = null;
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                    var ResponeTask = Client.GetAsync(GlobalHelpers.GetAdminDetails + id + "&orgID=" + organisationID);
                    ResponeTask.Wait();
                    var Result = ResponeTask.Result;
                    if (Result.IsSuccessStatusCode)
                    {
                        _Logger.Info("Details delivered.");
                        var ReadTask = Result.Content.ReadAsAsync<Admin>();
                        ReadTask.Wait();
                        admin = ReadTask.Result;
                    }
                }
                return View(admin);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        /// <summary>
        /// Admin SignOut
        /// </summary>
        /// <returns>Redirects to Login Page</returns>
        public ActionResult SignOut()
        {
            if (Session["Security"] != null)
            {
                return RedirectToAction("Index", "Employee", new { id = Session["Security"].ToString(), organisattionID= Session["Organisation"] });
            }
            try
            {
                _Logger.Info("Signout process initiated.");
                Session.Abandon();
                FormsAuthentication.SignOut();
                return RedirectToAction("Login");
            }

            catch (Exception ex)
            {
                _Logger.Info("Signout process failed.");
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        /// <summary>
        /// Consumes the API that returns the details of a particular employee
        /// </summary>
        /// <param name="id">Employee ID of the employee whose details we want to see</param>
        /// <returns></returns>
        public ActionResult Details(int id ,string userName, string organisationID)
        {
            _Logger.Info("Requesting details of employee with Employee ID= " + id.ToString());
            if (Session["Security"] != null)
            {
                return RedirectToAction("ERROR401", "Home");
            }
            if (Session["AdminSecurity"] == null || userName != Session["AdminSecurity"].ToString())
            {
                return RedirectToAction("ERROR401", "Home");
            }
            try
            {
                using (var Client = new HttpClient())
                {
                    if (organisationID == "0"){ organisationID = null; }                  
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
        /// Consumes the API that returns a list of employees based on several parameters like Gender Filter, Department Filter, Sort Order and Search String
        /// </summary>
        /// <param name="userName">UserName of admin for authentication</param>
        /// <param name="sortOrder">the field on which sorting occurs</param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString">contains the string on which search operation is performed</param>
        /// <param name="page">page number</param>
        /// <param name="departmentFilter">Department Filter</param>
        /// <param name="genderFilter">Gender Filter</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult Index(string userName,string organisationID, string sortOrder, string currentFilter, string searchString, int? page, string departmentFilter, string genderFilter)
        {
            if (Session["Security"] != null)
            {
                return RedirectToAction("ERROR401", "Home");
            }
            if (Session["AdminSecurity"] == null || userName!=Session["AdminSecurity"].ToString() || organisationID!=Session["Organisation"].ToString())
            {
                return RedirectToAction("ERROR401", "Home");
            }
            SetFiltersForMainGrid(ref sortOrder, ref searchString, ref departmentFilter, ref genderFilter);
            int LowerLimit;
            EmployeePagedModel Model;
            
            SetPaginationParameters(page, out LowerLimit, out Model);
            try
            {
                using (var Client = new HttpClient())
                {
                    if (organisationID == "0") { organisationID = ""; }
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                    EmployeeParameters Parameters = new EmployeeParameters(organisationID,sortOrder, searchString, departmentFilter, genderFilter, LowerLimit, 10);
                    var Result = Client.PostAsJsonAsync(GlobalHelpers.GetAllEmployees, Parameters).Result;
                    if (Result.IsSuccessStatusCode)
                    {
                        ProcessEmployeeDataForMainGrid(Model, Result);
                    }
                }
                _Logger.Info("Employee list loaded.");
                return View(Model);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }

        /// <summary>
        /// Consumes the API that adds new organisation with an admin
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAdmin(string userName)
        {
            if (Session["Security"] != null)
            {
                return RedirectToAction("ERROR401", "Home");
            }
            if (Session["AdminSecurity"] == null || userName != Session["AdminSecurity"].ToString())
            {
                return RedirectToAction("ERROR401", "Home");
            }
            return View();
        }
        /// <summary>
        /// Consumes the API that adds new organisation with an admin
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAdmin(Admin admin)
        {
            _Logger.Info("Adding new organisation with following details: " + JsonConvert.SerializeObject(admin).ToString());
            if (ModelState.IsValid)
            {
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                    var Response = Client.PostAsJsonAsync(GlobalHelpers.CreateAdmin, admin).Result;
                    if (Response.IsSuccessStatusCode)
                    {
                        var ReadTask = Response.Content.ReadAsAsync<Admin>();
                        ReadTask.Wait();
                        admin = ReadTask.Result;                        
                        return RedirectToAction("ViewOrganisations", "Admin", new { userName = Session["AdminSecurity"].ToString() , organisationID = Session["Organisation"] });
                    }
                    else if (Response.StatusCode.ToString() == "422")
                    {
                        ModelState.AddModelError("OrganisationId", "An Organisation is already registered with this ID.");
                        return View(admin);
                    }
                    else if (Response.StatusCode.ToString() == "555")
                    {
                        ModelState.AddModelError("AdminEmail", "Email ID already registered.");
                        return View(admin);
                    }
                    else return RedirectToAction("ERROR404", "Home");
                }
            }
            return View(admin);
        }       
        /// <summary>
        /// Consumes the API that shows the list of all the organisation that are using this application
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="organisationID"></param>
        /// <returns></returns>
        public ActionResult ViewOrganisations(string userName,string organisationID)
        {
            _Logger.Info("Fetching the list of all organisations.");
            if (Session["Security"] != null)
            {
                return RedirectToAction("ERROR401", "Home");
            }
            if (Session["AdminSecurity"] == null || userName != Session["AdminSecurity"].ToString() || organisationID != Session["Organisation"].ToString())
            {
                return RedirectToAction("ERROR401", "Home");
            }
            Admin admin = null;
            try
            {
                List<Admin> Organisations = new List<Admin>();
                using (var Client = new HttpClient())
                { 
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());                    
                    var Result = Client.PostAsJsonAsync(GlobalHelpers.GetAllOrganisations,admin).Result;
                    if (Result.IsSuccessStatusCode)
                    {
                        _Logger.Info("Organisation List Loaded.");
                        var ReadTask = Result.Content.ReadAsAsync<List<Admin>>();
                        ReadTask.Wait();
                        Organisations = ReadTask.Result;
                    }
                }
                return View(Organisations);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return RedirectToAction("ERROR404", "Home");
            }
        }
        /// <summary>
        /// To delete an organisation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="organisationID"></param>
        /// <returns></returns>
        public ActionResult DeleteOrganisation(string organisation,string organisationName,string username,string organisationID)
        {
            if (Session["Security"] != null)
            {
                return RedirectToAction("ERROR401", "Home");
            }
            if (Session["AdminSecurity"] == null || username != Session["AdminSecurity"].ToString() || organisationID != Session["Organisation"].ToString())
            {
                return RedirectToAction("ERROR401", "Home");
            }
            TempData["OrganisationName"]=organisationName;
            return View();
        }
        /// <summary>
        /// Consumes the API that deletes an organisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteOrganisation(string organisation)
        {
            try
            {
                _Logger.Info("Attempting to delete organisation with organsation ID: " + JsonConvert.SerializeObject(organisation).ToString());
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(GlobalHelpers.WebAPIURL);
                    Client.DefaultRequestHeaders.Add("API_KEY", ConfigurationManager.AppSettings["APIKey"].ToString());
                    var Result = Client.GetAsync(GlobalHelpers.DeleteOrganisation + organisation).Result;
                    if (Result.IsSuccessStatusCode)
                    {
                        _Logger.Info("Organisation with organisation ID " + organisation + " deleted.");
                        return RedirectToAction("ViewOrganisations", "Admin",new { username= Session["AdminSecurity"].ToString(), organisationID = Session["Organisation"].ToString() });
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

    }
}