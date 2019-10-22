namespace DataObjects
{
    public class EmployeeParameters
    {
        #region Data Members
        public string OrganisationId;
        public string SortOrder;
        public string SearchString;
        public string DepartmentFilter;
        public string GenderFilter;
        public int LowerLimit;
        public int UpperLimit;

        #endregion

        #region Constructor
        public EmployeeParameters(string organisationId,string sortOrder, string searchString, string departmentFilter, string genderFilter, int lowerLimit, int upperLimit)
        {
            OrganisationId = organisationId;
            SortOrder = sortOrder;
            SearchString = searchString;
            DepartmentFilter = departmentFilter;
            GenderFilter = genderFilter;
            LowerLimit = lowerLimit;
            UpperLimit = 10;
        }
        #endregion
    }
}
