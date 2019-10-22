using DataObjects;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace DataAccess
{
    public class DataAccessOperations
    {
        #region Data Members
        public static Employee _Employee = null;

        public static Admin _Admin = null;

        log4net.ILog _Logger = log4net.LogManager.GetLogger(typeof(DataAccessOperations));
        private string _ConnectionString { get; set; }

        #endregion

        #region Constructor
        public DataAccessOperations()
        {
            _ConnectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// To create employee list based on filters
        /// </summary>
        /// <param name="employeeList"></param>
        /// <param name="dataReader"></param>
        private void CreateEmployeeListBasedOnFilters(EmployeePagedModel employeeList, MySqlDataReader dataReader)
        {
            try
            {
                while (dataReader.Read())
                {
                    employeeList.Employees.Add(new DataObjects.Employee
                    {
                        OrganisationId=dataReader[0].ToString(),
                        Employee_Id = int.Parse(dataReader[1].ToString()),
                        Name = dataReader[2].ToString(),
                        DOB = DateTime.Parse(dataReader[3].ToString()),
                        Gender = dataReader[4].ToString(),
                        Joining_Date = DateTime.Parse(dataReader[5].ToString()),
                        Department = dataReader[6].ToString(),
                        Email = dataReader[7].ToString(),
                        Contact_Number = dataReader[8].ToString(),
                        Status=dataReader[9].ToString()
                    });
                }
                employeeList.PageNumber = 10;
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        employeeList.PageCount = int.Parse(dataReader[0].ToString());
                    }
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
        /// To get the list of all the organisations that are using this application
        /// </summary>
        /// <returns></returns>
        public List<Admin> GetOrganisationList()
        {
            _Logger.Info("DataAccessOperations.GetListByPagination() called.");
            List<Admin> Organisations = new List<Admin>();
            using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
            {
                Connection.Open();
                MySqlCommand Command = new MySqlCommand("GetAllAdmins", Connection);
                Command.CommandType = CommandType.StoredProcedure;
                MySqlDataReader DataReader = Command.ExecuteReader();
                while (DataReader.Read())
                {
                    Organisations.Add(new DataObjects.Admin
                    {
                        OrganisationID = DataReader[0].ToString(),
                        OrganisationName = DataReader[1].ToString(),
                        OrganisationAddress = DataReader[2].ToString(),
                        AdminEmail = DataReader[3].ToString()
                    });
                }
            }
            return Organisations;

        }
        /// <summary>
        /// To get list according to parameters like page,filters,search etc.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public EmployeePagedModel GetListByPagination(EmployeeParameters parameters)
        {
            _Logger.Info("DataAccessOperations.GetListByPagination() called.");
            EmployeePagedModel EmployeeList = new EmployeePagedModel();
            try
            {                
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("GetEmployeesWithFilters", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgId", parameters.OrganisationId ?? string.Empty);
                    Command.Parameters.AddWithValue("@searchstring", parameters.SearchString??string.Empty);
                    Command.Parameters.AddWithValue("@sortorder", parameters.SortOrder);
                    Command.Parameters.AddWithValue("@departmentfilter", parameters.DepartmentFilter);
                    Command.Parameters.AddWithValue("@genderfilter", parameters.GenderFilter);
                    Command.Parameters.AddWithValue("@lowerlimit", parameters.LowerLimit);
                    Command.Parameters.AddWithValue("@upperlimit", parameters.UpperLimit);
                    MySqlDataReader DataReader = Command.ExecuteReader();
                    EmployeeList.Employees = new List<Employee>();
                    CreateEmployeeListBasedOnFilters(EmployeeList, DataReader);
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
            return EmployeeList;
        }
        /// <summary>
        /// To get list according to parameters like page,filters,search etc.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public EmployeePagedModel GetActiveListByPagination(EmployeeParameters parameters)
        {
            _Logger.Info("DataAccessOperations.GetActiveListByPagination() called.");
            EmployeePagedModel EmployeeList = new EmployeePagedModel();
            try
            {
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("GetActiveEmployeesWithFilters", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgId", parameters.OrganisationId ?? string.Empty);
                    Command.Parameters.AddWithValue("@searchstring", parameters.SearchString ?? string.Empty);
                    Command.Parameters.AddWithValue("@sortorder", parameters.SortOrder);
                    Command.Parameters.AddWithValue("@departmentfilter", parameters.DepartmentFilter);
                    Command.Parameters.AddWithValue("@genderfilter", parameters.GenderFilter);
                    Command.Parameters.AddWithValue("@lowerlimit", parameters.LowerLimit);
                    Command.Parameters.AddWithValue("@upperlimit", parameters.UpperLimit);
                    MySqlDataReader DataReader = Command.ExecuteReader();
                    EmployeeList.Employees = new List<Employee>();
                    CreateEmployeeListBasedOnFilters(EmployeeList, DataReader);
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
            return EmployeeList;
        }
        /// <summary>
        /// To get employee details
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns></returns>
        public Employee GetEmployeeDetails(int id ,string orgID)
        {
            _Logger.Info("DataAccessOperations.GetEmployeeDetails() called.");
            try
            {                
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("GetEmployeeDetails", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@employeeid", id);
                    Command.Parameters.AddWithValue("@orgID", orgID);
                    MySqlDataReader DataReader = Command.ExecuteReader();
                    while (DataReader.Read())
                    {
                        _Employee = new Employee();
                       _Employee.Employee_Id = int.Parse(DataReader[0].ToString());
                       _Employee.Name = DataReader[1].ToString();
                       _Employee.DOB = DateTime.Parse(DataReader[2].ToString());
                       _Employee.Gender = DataReader[3].ToString();
                       _Employee.Joining_Date = DateTime.Parse(DataReader[4].ToString());
                       _Employee.Department = DataReader[5].ToString();
                       _Employee.Email = DataReader[6].ToString();
                       _Employee.Contact_Number = DataReader[7].ToString();
                       _Employee.OrganisationId = DataReader[8].ToString();
                       _Employee.Status = DataReader[9].ToString();
                        if(DataReader[10] != DBNull.Value)
                        {
                            _Employee.EndDate = DateTime.Parse(DataReader[10].ToString());
                        }                       
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
            return _Employee;
        }
        /// <summary>
        /// To get admin details
        /// </summary>
        /// <param name="id">Admin Username</param>
        /// <returns></returns>
        public Admin GetAdminDetails(string username, string orgID)
        {
            _Logger.Info("DataAccessOperations.GetAdminDetails() called.");
            try
            {
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("GetAdminDetails", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@username", username);
                    Command.Parameters.AddWithValue("@orgID", orgID);
                    MySqlDataReader DataReader = Command.ExecuteReader();
                    while (DataReader.Read())
                    {
                        _Admin = new Admin
                        {
                            Admin_User_Name = DataReader[0].ToString(),
                            OrganisationID = DataReader[1].ToString(),
                            OrganisationName = DataReader[2].ToString(),
                            OrganisationAddress = DataReader[3].ToString()
                        };
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
            return _Admin;
        }
        /// <summary>
        /// To insert a new employee
        /// </summary>
        /// <param name="employee">Employee Details</param>
        /// <returns></returns>
        public int InsertEmployee(Employee employee)
        {
            _Logger.Info("DataAccessOperations.InsertEmployee() called.");
            using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
            {
                DateTime EmployeeDateTime = employee.DOB;
                string sqlFormattedEmployeeDate = EmployeeDateTime.Date.ToString("yyyy-MM-dd ");
                DateTime EmployeeJoinDate = employee.Joining_Date;
                string sqlFormattedEmployeeJoinDate = EmployeeJoinDate.Date.ToString("yyyy-MM-dd");
                try
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("InsertEmployee", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@employeename", employee.Name);
                    Command.Parameters.AddWithValue("@dob", sqlFormattedEmployeeDate);
                    Command.Parameters.AddWithValue("@gender", employee.Gender);
                    Command.Parameters.AddWithValue("@joiningdate", sqlFormattedEmployeeJoinDate);
                    Command.Parameters.AddWithValue("@department", employee.Department);
                    Command.Parameters.AddWithValue("@email", employee.Email);
                    Command.Parameters.AddWithValue("@contact", employee.Contact_Number);
                    Command.Parameters.AddWithValue("@password", employee.Password);
                    Command.Parameters.AddWithValue("@orgId", employee.OrganisationId);
                    int CheckExecution = Command.ExecuteNonQuery();
                    return CheckExecution;
                }
                catch (Exception ex)
                {
                    _Logger.Fatal(ex.ToString());
                    throw ex;
                }
            }
        }
        /// <summary>
        /// To insert a new organisation with an admin
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public int InsertAdmin(Admin admin)
        {
            _Logger.Info("DataAccessOperations.InsertAdmin() called.");
            using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
            {
               
                try
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("InsertAdmin", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgID", admin.OrganisationID);
                    Command.Parameters.AddWithValue("@adminusername", admin.Admin_User_Name);
                    Command.Parameters.AddWithValue("@adminemail", admin.AdminEmail);
                    Command.Parameters.AddWithValue("@password", admin.Password);
                    Command.Parameters.AddWithValue("@orgName", admin.OrganisationName);
                    Command.Parameters.AddWithValue("@orgAddress", admin.OrganisationAddress);                    
                    int CheckExecution = Command.ExecuteNonQuery();
                    return CheckExecution;
                }
                catch (Exception ex)
                {
                    _Logger.Fatal(ex.ToString());
                    throw ex;
                }
            }
        }        
        /// <summary>
        /// To update an employee with Employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public int UpdateEmployee(Employee employee)
        {
            _Logger.Info("DataAccessOperations.UpdateEmployee() called.");
            try
            {                
                DateTime EmployeeDateTime = employee.DOB;
                string sqlFormattedEmployeeDate = EmployeeDateTime.Date.ToString("yyyy-MM-dd ");
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("UpdateEmployee", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgID", employee.OrganisationId);
                    Command.Parameters.AddWithValue("@employeeid", employee.Employee_Id);
                    Command.Parameters.AddWithValue("@employeename", employee.Name);
                    Command.Parameters.AddWithValue("@dob", sqlFormattedEmployeeDate);
                    Command.Parameters.AddWithValue("@gender", employee.Gender);
                    Command.Parameters.AddWithValue("@department", employee.Department);
                    Command.Parameters.AddWithValue("@email", employee.Email);
                    Command.Parameters.AddWithValue("@contact", employee.Contact_Number);
                    int CheckExecution = Command.ExecuteNonQuery();
                    return CheckExecution;
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
        public int UpdateEmployeeWithAdmin(Employee employee)
        {
            _Logger.Info("DataAccessOperations.UpdateEmployeeWithAdmin() called.");
            try
            {                
                DateTime EmployeeDateTime = employee.DOB;
                string sqlFormattedEmployeeDate = EmployeeDateTime.Date.ToString("yyyy-MM-dd ");
                DateTime EmployeeJoinTime = employee.Joining_Date;
                string sqlFormattedEmployeeJoinDate = EmployeeJoinTime.Date.ToString("yyyy-MM-dd ");
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("UpdateEmployeeWithAdmin", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgID", employee.OrganisationId);
                    Command.Parameters.AddWithValue("@employeeid", employee.Employee_Id);
                    Command.Parameters.AddWithValue("@employeename", employee.Name);
                    Command.Parameters.AddWithValue("@dob", sqlFormattedEmployeeDate);
                    Command.Parameters.AddWithValue("@gender", employee.Gender);
                    Command.Parameters.AddWithValue("@joiningdate", sqlFormattedEmployeeJoinDate);
                    Command.Parameters.AddWithValue("@department", employee.Department);
                    Command.Parameters.AddWithValue("@email", employee.Email);
                    Command.Parameters.AddWithValue("@contact", employee.Contact_Number);
                    int CheckExecution = Command.ExecuteNonQuery();
                    return CheckExecution;
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
            _Logger.Info("DataAccessOperations.ValidateEmployee() called.");
            try
            {                
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("CheckEmployeeLoginDetails", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgID", employee.OrganisationId);
                    Command.Parameters.AddWithValue("@employeeid", employee.Employee_Id);
                    Command.Parameters.AddWithValue("@password", employee.Password);
                    MySqlDataReader DataReader = Command.ExecuteReader();
                    if (DataReader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }                   
                }
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
            _Logger.Info("DataAccessOperations.ValidateAdmin() called.");
            try
            {               
                using (MySqlConnection Conncetion = new MySqlConnection(_ConnectionString))
                {
                    Conncetion.Open();
                    MySqlCommand Command = new MySqlCommand("CheckAdminLoginDetails", Conncetion);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgID", admin.OrganisationID);
                    Command.Parameters.AddWithValue("@username", admin.Admin_User_Name);
                    Command.Parameters.AddWithValue("@password", admin.Password);
                    MySqlDataReader DataReader = Command.ExecuteReader();
                    if (DataReader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
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
        public int ChangeAdminPassword(Admin admin)
        {
            _Logger.Info("DataAccessOperations.ChangeAdminPassword() called.");
            try
            {                
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("ChangeAdminPassword", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgID", admin.OrganisationID);
                    Command.Parameters.AddWithValue("@username", admin.Admin_User_Name);
                    Command.Parameters.AddWithValue("@oldpassword", admin.OrganisationName);
                    Command.Parameters.AddWithValue("@newpassword", admin.Password);
                    int CheckExecution = Command.ExecuteNonQuery();
                    Connection.Close();
                    return CheckExecution;
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
        public int ChangeEmployeePassword(Employee employee)
        {
            _Logger.Info("DataAccessOperations.ChangeEmployeePassword() called.");
            try
            {                
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("ChangeEmployeePassword", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgID", employee.OrganisationId);
                    Command.Parameters.AddWithValue("@employeeid", employee.Employee_Id);
                    Command.Parameters.AddWithValue("@oldpassword", employee.Name);
                    Command.Parameters.AddWithValue("@newpassword", employee.Password);
                    int CheckExecution = Command.ExecuteNonQuery();
                    Connection.Close();
                    return CheckExecution;
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To change the status of an employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public int ChangeStatus(Employee employee)
        {
            _Logger.Info("DataAccessOperations.ChangeStatus() called.");
            try
            {
                DateTime EmployeeDateTime = (DateTime)employee.EndDate;
                string sqlFormattedEmployeeDate = EmployeeDateTime.Date.ToString("yyyy-MM-dd ");
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("ChangeStatus", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgID", employee.OrganisationId);
                    Command.Parameters.AddWithValue("@employeeid", employee.Employee_Id);
                    Command.Parameters.AddWithValue("@employeestatus", employee.Status);
                    Command.Parameters.AddWithValue("@lastdate", sqlFormattedEmployeeDate);
                    int CheckExecution = Command.ExecuteNonQuery();
                    Connection.Close();
                    return CheckExecution;
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To get the employeeId of newly added employee
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        public int GetNewEmployeeId(string organisationId)
        {
            _Logger.Info("DataAccessOperations.GetNewEmployeeId() called.");
            try
            {
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    int NewEmployeeId = 0;
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("GetEmployeeId", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgID",organisationId);
                    MySqlDataReader DataReader = Command.ExecuteReader();
                    while (DataReader.Read())
                    {
                        NewEmployeeId = int.Parse(DataReader[0].ToString());
                    }
                    return NewEmployeeId;
                }
            }
            catch (Exception ex)
            {
                _Logger.Fatal(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// To get OrganisationId of newly added organisation
        /// </summary>
        /// <returns></returns>
        public int GetNewOrganisationId()
        {
            _Logger.Info("DataAccessOperations.GetNewOrganisationId() called.");
            try
            {
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    int NewOrganisationId = 0;
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("GetOrganisationId", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    MySqlDataReader DataReader = Command.ExecuteReader();
                    while (DataReader.Read())
                    {
                        NewOrganisationId = int.Parse(DataReader[0].ToString());
                    }
                    return NewOrganisationId;
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
            _Logger.Info("DataAccessOperations.DeleteOrganisation() called to delete organisaiton with organisation ID" + id.ToString());
            try
            {
                using (MySqlConnection Connection = new MySqlConnection(_ConnectionString))
                {
                    Connection.Open();
                    MySqlCommand Command = new MySqlCommand("DeleteOrganisation", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@orgID", id);
                    int Check = Command.ExecuteNonQuery();
                    if (Check == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
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
