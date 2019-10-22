using System;

namespace EMS.ExceptionHandling
{
    public class InvalidEmployeeIDException : EmployeeManagementExceptions
    {
        public InvalidEmployeeIDException()
        { }
        public InvalidEmployeeIDException(string id) : base(String.Format("Invalid Employee ID : " + id))
        { }
    }
    public class InvalidEmployeePasswordException : EmployeeManagementExceptions
    {
        public InvalidEmployeePasswordException()
        { }
        public InvalidEmployeePasswordException(string password) : base(String.Format("Invalid Employee Password: " + password))
        { }
    }
    public class InvalidEmployeeGenderException : EmployeeManagementExceptions
    {
        public InvalidEmployeeGenderException()
        { }
        public InvalidEmployeeGenderException(string gender) : base(String.Format("Invalid Employee Gender: " + gender + ". Employee Gender should be of length 4-6"))
        { }
    }
    public class InvalidEmployeeEmailException : EmployeeManagementExceptions
    {
        public InvalidEmployeeEmailException()
        { }
        public InvalidEmployeeEmailException(string email) : base(String.Format("Invalid Employee Email: " + email))
        { }
    }
    public class InvalidEmployeeDepartmentException : Exception
    {
        public InvalidEmployeeDepartmentException()
        { }
        public InvalidEmployeeDepartmentException(string department) : base(String.Format("Invalid Department: " + department))
        { }
    }
    public class InvalidEmployeeContactNumberException : EmployeeManagementExceptions
    {
        public InvalidEmployeeContactNumberException()
        { }
        public InvalidEmployeeContactNumberException(string ContactNumber) : base(String.Format("Invalid Employee Contact: " + ContactNumber + ". Employee contact number should be of length 10"))
        { }
    }
    public class InvalidEmployeeNameException : EmployeeManagementExceptions
    {
        public InvalidEmployeeNameException()
        { }
        public InvalidEmployeeNameException(string name) : base(String.Format("Invalid Employee Name: " + name + ". Employee name must not have a length greater than 30."))
        { }
    }
}
