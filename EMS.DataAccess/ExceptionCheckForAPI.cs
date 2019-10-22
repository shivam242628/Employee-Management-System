using DataAccess;
using DataObjects;
using EMS.ExceptionHandling;
using System.Text.RegularExpressions;

namespace EMS.Business
{
    public class ExceptionCheckForAPI
    {

        #region Data Members
        DataAccessOperations _EmployeeOperations = new DataAccessOperations();

        log4net.ILog _Logger = log4net.LogManager.GetLogger(typeof(BusinessOperations));
        #endregion

        #region public functions for validating API inputs
        public void ValidateAdminPassword(string password)
        {
            if (password.Length >= 6 && new Regex("^([a-zA-Z0-9 .&-]+)$").IsMatch(password))
            {
                
            }
            else
            {
                throw new InvalidAdminPasswordException(password);
            }
        }

        public void ValidateAdminUsername(string userName)
        {
            if (userName.Length>30)
            {
                throw new InvalidAdminUserNameException(userName);
            }
        }

        public void ValidateEmployeeID(int id)
        {
            if (id <= 0 || id > 700000)
            {
                throw new InvalidEmployeeIDException(id.ToString());
            }
        }

        public void ValidateEmployeeIDSyntax(int id)
        {
            if (id <= 0 || id > 700000)
            {
                throw new InvalidEmployeeIDException(id.ToString());
            }
        }

        public void ValidateEmployeePassword(string password)
        {
            if (password.Length >= 6 && new Regex("^([a-zA-Z0-9 .&-]+)$").IsMatch(password))
            {

            }
            else
            {
                throw new InvalidEmployeePasswordException(password);
            }
        }

        public void ValidateEmployee(Employee employee)
        {
            if (employee.Employee_Id < 0 || employee.Employee_Id > 700000)
            {
                throw new InvalidEmployeeIDException(employee.Employee_Id.ToString());
            }

            if (employee.Name.Length > 30)
            {
                throw new InvalidEmployeeNameException(employee.Name);
            }

            if (employee.Gender.Length < 4 || employee.Gender.Length > 6)
            {
                throw new InvalidEmployeeGenderException(employee.Gender);
            }

            if (!Regex.IsMatch(employee.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
            {
                throw new InvalidEmployeeEmailException(employee.Email);
            }

            if (employee.Department.Length < 2 || employee.Department.Length > 20)
            {
                throw new InvalidEmployeeDepartmentException(employee.Department);
            }


            if (employee.Password != null)
            {
                if (employee.Password.Length < 6)
                {
                    throw new InvalidEmployeePasswordException(employee.Password);
                }
            }
        }

        public void ValidateOrganisation(Admin admin)
        {
            if ((int.Parse(admin.OrganisationID) < 0 && int.Parse(admin.OrganisationID) > 70000))
            {
                throw new InvalidOrganisationIdException(admin.OrganisationID);
            }

            if(admin.OrganisationName.Length > 20)
            {
                throw new InvalidOrganisationNameException(admin.OrganisationName);
            }

            if (admin.OrganisationAddress.Length > 100)
            {
                throw new InvalidOrganisationAddressException(admin.OrganisationAddress);
            }

            if (admin.AdminEmail.Length > 50)
            {
                throw new InvalidOrganisationEmailException(admin.AdminEmail);
            }
        }

        #endregion
    }
}
