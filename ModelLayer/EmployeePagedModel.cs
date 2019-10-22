using System.Collections.Generic;

namespace DataObjects
{
    public class EmployeePagedModel
    {
        #region Data Members
        public List<Employee> Employees { get; set; }

        public int PageNumber { get; set; }

        public int PageCount { get; set; }

        #endregion
    }
}
