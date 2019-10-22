using System;

namespace EMS.ExceptionHandling
{
    public class InvalidAdminUserNameException : EmployeeManagementExceptions
    {
        public InvalidAdminUserNameException()
        { }
        public InvalidAdminUserNameException(string username) : base(String.Format("Invalid Admin Username : " + username))
        { }
    }

    public class InvalidAdminPasswordException : EmployeeManagementExceptions
    {
        public InvalidAdminPasswordException()
        { }
        public InvalidAdminPasswordException(string password) : base(String.Format("Invalid Admin Password: " + password + ". Admin Password should not contain any special characters and must have a minimum length of 6."))
        { }
    }

    public class InvalidOrganisationIdException : EmployeeManagementExceptions
    {
        public InvalidOrganisationIdException()
        { }
        public InvalidOrganisationIdException(string organisationId) : base(String.Format("Invalid Organisation ID : " + organisationId))
        { }
    }

    public class InvalidOrganisationNameException : EmployeeManagementExceptions
    {
        public InvalidOrganisationNameException()
        { }
        public InvalidOrganisationNameException(string organisationName) : base(String.Format("Invalid Organisation Name: " + organisationName + ". Orgnaisation name must not have a length greater than 20."))
        { }
    }

    public class InvalidOrganisationAddressException : EmployeeManagementExceptions
    {
        public InvalidOrganisationAddressException()
        { }
        public InvalidOrganisationAddressException(string organisationAddress) : base(String.Format("Invalid Organisation Address: " + organisationAddress + ". Orgnaisation Address must not have a length greater than 100."))
        { }
    }

    public class InvalidOrganisationEmailException : EmployeeManagementExceptions
    {
        public InvalidOrganisationEmailException()
        { }
        public InvalidOrganisationEmailException(string organisationEmail) : base(String.Format("Invalid Organisation Email: " + organisationEmail + ". Orgnaisation Email must not have a length greater than 50."))
        { }
    }
}
