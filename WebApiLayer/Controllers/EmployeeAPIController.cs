using EMS.Business;
using DataObjects;
using System;
using System.Web.Http;
using EMS.ExceptionHandling;
using MySql.Data.MySqlClient;
using System.Net;
using System.Collections.Generic;

namespace WebApiLayer.Controllers
{
    public class EmployeeAPIController : ApiController
    {
        #region Data Members

        public static Employee _Employee = null;

        log4net.ILog _Logger = log4net.LogManager.GetLogger(typeof(EmployeeAPIController));

        BusinessOperations _BusinessOperations = new BusinessOperations();

        ExceptionCheckForAPI _ExceptionCheck = new ExceptionCheckForAPI();

        #endregion

        #region GET APIs
        /// <summary>
        /// API to get details of a particular employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns></returns>
        public IHttpActionResult GetEmployeeDetails(int id,string orgID)
        {
            _Logger.Info("GetEmployeeDetails API called.");
            try
            {                
                _ExceptionCheck.ValidateEmployeeIDSyntax(id);
                _Employee = _BusinessOperations.GetEmployeeDetails(id,orgID);
                if (_Employee == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(_Employee);
                }
            }

            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.ToString());
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// API to get details of a particular admin
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns></returns>
        public IHttpActionResult GetAdminDetails(string username, string orgID)
        {
            _Logger.Info("GetAdminDetails API called.");
            try
            {
                _ExceptionCheck.ValidateAdminUsername(username);
                Admin admin = _BusinessOperations.GetAdminDetails(username, orgID);
                if (admin == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(admin);
                }
            }

            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.ToString());
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// API to delete an organisation by super admin
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public IHttpActionResult GetDeleteOrganisation(string organisation)
        {
            _Logger.Info("PostDeleteOrganisation API called.");
            try
            {
                bool Check = _BusinessOperations.DeleteOrganisation(organisation);
                if (Check == true)
                    return Ok();
                else return InternalServerError();
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }

        #endregion

        #region POST APIs
        /// <summary>
        /// API to GET details of all the organisations
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public IHttpActionResult PostAllOrganisations(Admin admin)
        {
            _Logger.Info("PostAllOrganisations API called.");
            try
            {
                List<Admin> OrganisationList = new List<Admin>();
                OrganisationList = _BusinessOperations.GetOrganisationList();
                return Ok(OrganisationList);

            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }        
        /// <summary>
        /// API to GET all employees based on filters and Sort Order
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IHttpActionResult PostAllEmployees(EmployeeParameters parameters)
        {
            _Logger.Info("PostAllEmployees API called.");
            try
            {                
                EmployeePagedModel EmployeeList = new EmployeePagedModel();
                EmployeeList = _BusinessOperations.GetListByPagination(parameters);
                return Ok(EmployeeList);
                
            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// API to GET all employees based on filters and Sort Order
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IHttpActionResult PostAllActiveEmployees(EmployeeParameters parameters)
        {
            _Logger.Info("PostAllActiveEmployees API called.");
            try
            {
                EmployeePagedModel EmployeeList = new EmployeePagedModel();
                EmployeeList = _BusinessOperations.GetActiveListByPagination(parameters);
                return Ok(EmployeeList);

            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// API to edit a particular employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public IHttpActionResult PostEditEmployee(Employee employee)
        {
            _Logger.Info("PostEditEmployee API called.");
            try
            {                
                _ExceptionCheck.ValidateEmployee(employee);
                bool Check = _BusinessOperations.UpdateEmployee(employee);
                if (Check == false)
                {
                    _Logger.Error("Employee Not Updated");
                    return base.NotFound();
                }
                else return base.Ok(EmployeeAPIController._Employee);
            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (MySqlException exception)
            {
                _Logger.Error(exception.ToString());
                if (exception.Message.Contains("Duplicate"))
                    return Content((HttpStatusCode)422, "Database Error");
                else return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// API to change status of employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public IHttpActionResult PostChangeStatus(Employee employee)
        {
            _Logger.Info("PostEditEmployee API called.");
            try
            {
                bool Check = _BusinessOperations.ChangeStatus(employee);
                if (Check == false)
                    return NotFound();
                else return Ok(employee);
            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// API to edit an employee with admin
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public IHttpActionResult PostEditEmployeeWithAdmin(Employee employee)
        {
            _Logger.Info("PostEditEmployeeWithAdmin API called.");
            try
            {                
                _ExceptionCheck.ValidateEmployee(employee);
                bool Check = _BusinessOperations.UpdateEmployeeWithAdmin(employee);
                if (Check == false)
                {
                _Logger.Error("Employee Not Updated");
                return base.NotFound();
                }
                else return base.Ok(EmployeeAPIController._Employee);          
            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (MySqlException exception)
            {
                _Logger.Error(exception.ToString());
                return Content((HttpStatusCode)422, "Database Error");
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// API to change employee password
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public IHttpActionResult PostChangeEmployeePassword(Employee employee)
        {
            _Logger.Info("PostChangeEmployeePassword API called.");
            try
            {                
                _ExceptionCheck.ValidateEmployeePassword(employee.Password);
                bool Check = _BusinessOperations.ChangeEmployeePassword(employee);
                if (Check == false)
                    return NotFound();
                else return Ok(employee);
            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// API to change admin password
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public IHttpActionResult PostChangeAdminPassword(Admin admin)
        {
            _Logger.Info("PostChangeAdminPassword API called.");
            try
            {                
                _ExceptionCheck.ValidateAdminPassword(admin.Password);
                bool Check = _BusinessOperations.ChangeAdminPassword(admin);
                if (Check == false)
                    return NotFound();
                else return Ok(admin);
            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// API to insert a new employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public IHttpActionResult PostEmployee(Employee employee)
        {
            _Logger.Info("PostEmployee API called.");
            try
            {                
                _ExceptionCheck.ValidateEmployee(employee);
                _Employee= _BusinessOperations.InsertEmployee(employee);
                if (_Employee.Employee_Id == 0)
                {
                _Logger.Error("Employee Not Added");
                return base.NotFound();
                }
                else return base.Ok(EmployeeAPIController._Employee);
            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (MySqlException exception)
            {
                _Logger.Error(exception.ToString());
                if (exception.Message.Contains("child"))
                    return Content((HttpStatusCode)555, "Database Error");
                if (exception.Message.Contains("Duplicate"))
                    return Content((HttpStatusCode)422, "Database Error");
                else return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// API to create a new admin corresponding to a new organisation.
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public IHttpActionResult PostCreateAdmin(Admin admin)
        {
            _Logger.Info("PostCreateAdmin API called.");
            try
            {
                _ExceptionCheck.ValidateOrganisation(admin);
                admin = _BusinessOperations.InsertAdmin(admin);
                if (admin.OrganisationID == "0")
                {
                    _Logger.Error("Admin Not Added");
                    return base.NotFound();
                }
                else return base.Ok(admin);
            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (MySqlException exception)
            {
                _Logger.Error(exception.ToString());
                if (exception.Message.Contains("adminemail"))
                    return Content((HttpStatusCode)555, "Database Error");
                if (exception.Message.Contains("Duplicate"))
                    return Content((HttpStatusCode)422, "Database Error");
                else return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// API to check employee login details
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public IHttpActionResult PostCheckEmployeeLoginDetails(Employee employee)
        {
            _Logger.Info("PostCheckEmployeeLoginDetails API called.");
            try
            {                
                _ExceptionCheck.ValidateEmployeeID(employee.Employee_Id);
                _ExceptionCheck.ValidateEmployeePassword(employee.Password);
                bool LoginCheck = _BusinessOperations.ValidateEmployee(employee);
                if (LoginCheck == true)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// API to check admin login details
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public IHttpActionResult PostCheckAdminLoginDetails(Admin admin)
        {
            _Logger.Info("PostCheckAdminLoginDetails API called.");
            try
            {                
                _ExceptionCheck.ValidateAdminUsername(admin.Admin_User_Name);
                _ExceptionCheck.ValidateAdminPassword(admin.Password);
                bool LoginCheck = _BusinessOperations.ValidateAdmin(admin);
                if (LoginCheck == true)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (EmployeeManagementExceptions exception)
            {
                _Logger.Error(exception.ToString());
                return InternalServerError(exception);
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                return InternalServerError(ex);
            }
        }

        #endregion
    }
}

