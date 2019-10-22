namespace Employee_Management_Services.GlobalObjects
{

    public static class GlobalHelpers
    {
        public static string WebAPIURL = "";

        #region APIEndpoints

        public static readonly string GetEmployeeDetails = "Api/EmployeeAPI/GetEmployeeDetails?id=";

        public static readonly string GetAdminDetails = "Api/EmployeeAPI/GetAdminDetails?username=";

        public static readonly string GetAllEmployees = "Api/EmployeeAPI/PostAllEmployees";

        public static readonly string GetAllOrganisations = "Api/EmployeeAPI/PostAllOrganisations";

        public static readonly string DeleteOrganisation = "Api/EmployeeAPI/GetDeleteOrganisation?organisation=";

        public static readonly string GetAllActiveEmployees = "Api/EmployeeAPI/PostAllActiveEmployees";

        public static readonly string CreateEmployee = "Api/EmployeeAPI/PostEmployee";

        public static readonly string CreateAdmin = "Api/EmployeeAPI/PostCreateAdmin";

        public static readonly string EditEmployeeWithAdmin = "Api/EmployeeAPI/PostEditEmployeeWithAdmin";

        public static readonly string ChangeStatus = "Api/EmployeeAPI/PostChangeStatus";

        public static readonly string EditEmployee = "Api/EmployeeAPI/PostEditEmployee";

        public static readonly string DeleteEmployee = "Api/EmployeeAPI/PostDeleteEmployee";

        public static readonly string ValidateAdmin = "Api/EmployeeAPI/PostCheckAdminLoginDetails";

        public static readonly string ValidateEmployee = "Api/EmployeeAPI/PostCheckEmployeeLoginDetails";

        public static readonly string ChangeAdminPassword = "Api/EmployeeAPI/PostChangeAdminPassword";

        public static readonly string ChangeEmployeePassword = "Api/EmployeeAPI/PostChangeEmployeePassword";

        #endregion       

    }
}