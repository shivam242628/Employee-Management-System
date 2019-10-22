using DataObjects;
using DataAccess;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EMS.Business
{
    public class BusinessOperations
    {
        #region Data Members

        public static Employee _Employee = null;

        DataAccessOperations _EmployeeOperations = new DataAccessOperations();

        log4net.ILog _Logger = log4net.LogManager.GetLogger(typeof(BusinessOperations));

        #endregion

        #region Private Functions

        /// <summary>
        /// To send an email to employee who has been created by the organisation admin
        /// </summary>
        /// <param name="employee"></param>
        private async Task SendEmailToEmployee(Employee employee)
        {
            try
            {
                _Logger.Info("Send Email to created employee.");
                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("employeemanagementcosmos@gmail.com", "From EMS");
                var subject = "Welcome to Employee Management System";
                var to = new EmailAddress(employee.Email, "To " + employee.Name);
                var plainTextContent = "";
                var htmlContent = "Organisation Id : " + employee.OrganisationId + "    Employee Id : " + employee.Employee_Id + "    Password : " + employee.Password;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                dynamic response = await client.SendEmailAsync(msg);
                _Logger.Info("Email Sent.");
            }
            catch (Exception ex)
            {
                _Logger.Error("Email not sent.");
                throw ex;
            }
        }
        /// <summary>
        /// To send an email to the organisation admin who has been created by the superadmin
        /// </summary>
        /// <param name="admin"></param>
        private async Task SendEmailToAdmin(Admin admin)
        {
            try
            {
                _Logger.Info("Send Email to created organisation admin.");
                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("employeemanagementcosmos@gmail.com", "From EMS");
                var subject = "Welcome to Employee Management System";
                var to = new EmailAddress(admin.AdminEmail, "To " + admin.OrganisationName);
                var plainTextContent = "";
                var htmlContent = "Organisation Id : " + admin.OrganisationID + "    User Name : " + admin.Admin_User_Name + "    Password : " + admin.Password;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                dynamic response = client.SendEmailAsync(msg);
                _Logger.Info("Email Sent.");
            }
            catch (Exception ex)
            {
                _Logger.Error("Email not Sent.");
                throw ex;
            }
        }

        #endregion

        #region Public Functions
        /// <summary>
        /// To get the list of all thve organisations
        /// </summary>
        /// <returns></returns>
        public List<Admin> GetOrganisationList()
        {
            _Logger.Info("BusinessOperations.GetOrganisationList() called.");
            try
            {
                List<Admin> OrganisationList = new List<Admin>();
                OrganisationList = _EmployeeOperations.GetOrganisationList();
                return OrganisationList;
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To get list according to parameters like page,filters,search etc.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public EmployeePagedModel GetListByPagination(EmployeeParameters parameters)
        {
            _Logger.Info("BusinessOperations.GetListByPagination() called." + JsonConvert.SerializeObject(parameters));
            try
            {                
                if (parameters.SearchString == null)
                { }
                else if ((new Regex("^([&'-]+)$").IsMatch(parameters.SearchString)))
                {
                    parameters.SearchString = null;
                }
                EmployeePagedModel EmployeeList = new EmployeePagedModel();
                EmployeeList = _EmployeeOperations.GetListByPagination(parameters);
                return EmployeeList;
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To get list according to parameters like page,filters,search etc.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public EmployeePagedModel GetActiveListByPagination(EmployeeParameters parameters)
        {
            _Logger.Info("BusinessOperations.GetActiveListByPagination() called." + JsonConvert.SerializeObject(parameters));
            try
            {
                if (parameters.SearchString == null)
                { }
                else if ((new Regex("^([&'-]+)$").IsMatch(parameters.SearchString)))
                {
                    parameters.SearchString = null;
                }
                EmployeePagedModel EmployeeList = new EmployeePagedModel();
                EmployeeList = _EmployeeOperations.GetActiveListByPagination(parameters);
                return EmployeeList;
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To get employee details
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns></returns>
        public Employee GetEmployeeDetails(int id,string orgID)
        {
            _Logger.Info("BusinessOperations.GetEmployeeDetails() called for Employee ID " + id.ToString());
            try
            {                
                _Employee = _EmployeeOperations.GetEmployeeDetails(id,orgID);
                return _Employee;
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To get admin details
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns></returns>
        public Admin GetAdminDetails(string username, string orgID)
        {
            _Logger.Info("BusinessOperations.GetAdminDetails() called for Admin UserName " + username);
            try
            {
                Admin admin = _EmployeeOperations.GetAdminDetails(username, orgID);
                return admin;
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To insert a new employee
        /// </summary>
        /// <param name="employee">Employee Details</param>
        /// <returns></returns>
        public Employee InsertEmployee(Employee employee)
        {
            _Logger.Info("BusinessOperations.InsertEmployee() called for employee with employee details: " + JsonConvert.SerializeObject(employee).ToString());
            try
            {                
                int Check = _EmployeeOperations.InsertEmployee(employee);
                int NewEmployeeId = _EmployeeOperations.GetNewEmployeeId(employee.OrganisationId);
                employee.Employee_Id = NewEmployeeId;
                Task.Run(() => SendEmailToEmployee(_Employee));
                return employee;
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To insert new organisation with an admin
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public Admin InsertAdmin(Admin admin)
        {
            _Logger.Info("BusinessOperations.InsertAdmin() called for organisation with organisation details: " + JsonConvert.SerializeObject(admin).ToString());
            try
            {
                int Check = _EmployeeOperations.InsertAdmin(admin);
                int NewOrganisationId = _EmployeeOperations.GetNewOrganisationId();
                admin.OrganisationID = NewOrganisationId.ToString();
                Task.Run(() => SendEmailToAdmin(admin));
                return admin;
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }        
        /// <summary>
        /// To update an employee with Employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public bool UpdateEmployee(Employee employee)
        {
            _Logger.Info("BusinessOperations.UpdateEmployee() called with employee details: " + JsonConvert.SerializeObject(employee).ToString());
            try
            {                
                int Check = _EmployeeOperations.UpdateEmployee(employee);
                if (Check >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To update an employee with Admin
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public bool UpdateEmployeeWithAdmin(Employee employee)
        {
            _Logger.Info("BusinessOperations.UpdateEmployeeWithAdmin() called for employee with details: " + JsonConvert.SerializeObject(employee).ToString());
            try
            {                
                int Check = _EmployeeOperations.UpdateEmployeeWithAdmin(employee);
                if (Check >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To validate the employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public bool ValidateEmployee(Employee employee)
        {
            _Logger.Info("BusinessOperations.ValidateEmployee() called.");
            try
            {                
                bool LoginCheck = _EmployeeOperations.ValidateEmployee(employee);
                return LoginCheck;
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To validate the admin
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public bool ValidateAdmin(Admin admin)
        {
            _Logger.Info("BusinessOperations.ValidateAdmin() called.");
            try
            {                
                bool LoginCheck = _EmployeeOperations.ValidateAdmin(admin);
                return LoginCheck;
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To change admin password
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public bool ChangeAdminPassword(Admin admin)
        {
            _Logger.Info("BusinessOperations.ChangeAdminPassword() called.");
            try
            {                
                int Check = _EmployeeOperations.ChangeAdminPassword(admin);
                if (Check >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To change employee password
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public bool ChangeEmployeePassword(Employee employee)
        {
            _Logger.Info("BusinessOperations.ChangeEmployeePassword() called.");
            try
            {                
                int Check = _EmployeeOperations.ChangeEmployeePassword(employee);
                if (Check >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To change status of an employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public bool ChangeStatus(Employee employee)
        {
            _Logger.Info("BusinessOperations.ChangeStatus() called");
            try
            {
                int Check = _EmployeeOperations.ChangeStatus(employee);
                if (Check >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To delete an organisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteOrganisation(string id)
        {
            _Logger.Info("BusinessOperations.DeleteOrganisation() called");
            try
            {
                bool Check = _EmployeeOperations.DeleteOrganisation(id);
                if (Check==true)
                {
                    return true;
                }
                else
                {
                    return false;
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
