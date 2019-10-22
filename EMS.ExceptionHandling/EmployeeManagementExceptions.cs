using System;

namespace EMS.ExceptionHandling
{
    public class EmployeeManagementExceptions : Exception
    {
        public EmployeeManagementExceptions() : base() { }
        public EmployeeManagementExceptions(string message) : base(message) { }
    }
}
