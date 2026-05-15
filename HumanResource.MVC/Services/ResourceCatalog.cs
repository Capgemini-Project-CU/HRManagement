using HumanResource.MVC.Models.Resources;

namespace HumanResource.MVC.Services;

public class ResourceCatalog
{
    private readonly List<ApiResourceDefinition> _resources;

    public ResourceCatalog()
    {
        _resources =
        [
            Employees(),
            Departments(),
            Jobs(),
            JobHistory(),
            Roles(),
            Regions(),
            Countries(),
            Locations()
        ];
    }

    public IReadOnlyList<ApiResourceDefinition> All => _resources;

    public ApiResourceDefinition? Find(string key)
    {
        return _resources.FirstOrDefault(resource =>
            string.Equals(resource.Key, key, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsAllowed(string role, IEnumerable<string> allowedRoles)
    {
        return allowedRoles.Any(allowedRole =>
            string.Equals(allowedRole, role, StringComparison.OrdinalIgnoreCase));
    }

    private static ApiResourceDefinition Employees()
    {
        return new ApiResourceDefinition
        {
            Key = "employees",
            Title = "Employees",
            Endpoint = "api/Employees",
            IdField = "employeeId",
            Icon = "",
            ViewRoles = ["Admin", "HR"],
            CreateRoles = ["Admin", "HR"],
            EditRoles = ["Admin", "HR"],
            DeleteRoles = ["Admin"],
            Fields =
            [
                Number("employeeId", "Employee", showInCreate: false, showInEdit: false, includeInCreatePayload: false),
                Text("firstName", "First name", required: true),
                Text("lastName", "Last name", required: true),
                Text("email", "Email", required: true),
                Password("password", "Password", required: true, showInTable: false, showInEdit: false, includeInEditPayload: false),
                Text("phoneNumber", "Phone"),
                Date("hireDate", "Hire date", required: true, showInCreate: false, includeInCreatePayload: false),
                Number("salary", "Salary", required: true),
                Select("managerId", "Manager", "employees"),
                Select("departmentId", "Department", "departments", required: true),
                Select("jobId", "Job", "jobs", required: true, type: ApiFieldType.Text),
                Select("roleId", "Role", "roles", required: true)
            ],
            Filters =
            [
                Filter("search", "Search", "api/Employees/search?keyword={keyword}", ["Admin", "HR"], Text("keyword", "Keyword", required: true)),
                Filter("department", "By department", "api/Employees/department/{departmentId}", ["Admin", "HR", "Employee"], Select("departmentId", "Department", "departments", required: true)),
                Filter("manager", "By manager", "api/Employees/manager/{managerId}", ["Admin", "HR"], Select("managerId", "Manager", "employees", required: true)),
                Filter("job", "By job", "api/Employees/job/{jobId}", ["Admin", "HR", "Employee"], Select("jobId", "Job", "jobs", required: true, type: ApiFieldType.Text)),
                Filter("role", "By role", "api/Employees/role/{roleId}", ["Admin"], Select("roleId", "Role", "roles", required: true))
            ]
        };
    }

    private static ApiResourceDefinition Departments()
    {
        return new ApiResourceDefinition
        {
            Key = "departments",
            Title = "Departments",
            Endpoint = "api/Departments",
            IdField = "departmentId",
            Icon = "",
            Summary = "Department structure with manager and location details.",
            ViewRoles = ["Admin", "HR", "Employee"],
            CreateRoles = ["Admin", "HR"],
            EditRoles = ["Admin", "HR"],
            DeleteRoles = ["Admin"],
            Fields =
            [
                Number("departmentId", "Department", showInTable: false),
                Text("departmentName", "Department name", required: true),
                Select("managerId", "Manager", "employees", showInTable: false),
                Select("locationId", "Location", "locations", showInTable: false),
                Text("managerName", "Manager", showInTable: true, readOnly: true),
                Text("city", "City", showInTable: true, readOnly: true)
            ],
            Filters =
            [
                Filter("location", "By location", "api/Departments/location/{locationId}", ["Admin", "HR", "Employee"], Select("locationId", "Location", "locations", required: true))
            ]
        };
    }

    private static ApiResourceDefinition Jobs()
    {
        return new ApiResourceDefinition
        {
            Key = "jobs",
            Title = "Jobs",
            Endpoint = "api/Jobs",
            IdField = "jobId",
            Icon = "",
            Summary = "Job titles and salary bands.",
            ViewRoles = ["Admin", "HR", "Employee"],
            CreateRoles = ["Admin", "HR"],
            EditRoles = ["Admin", "HR"],
            DeleteRoles = ["Admin"],
            Fields =
            [
                Text("jobId", "Job code", required: true, showInTable: false),
                Text("jobTitle", "Job title", required: true),
                Number("minSalary", "Min salary"),
                Number("maxSalary", "Max salary")
            ],
            Filters =
            [
                Filter("salary-range", "Salary range", "api/Jobs/salary-range?min={min}&max={max}", ["Admin", "HR", "Employee"], Number("min", "Minimum", required: true), Number("max", "Maximum", required: true))
            ]
        };
    }

    private static ApiResourceDefinition JobHistory()
    {
        return new ApiResourceDefinition
        {
            Key = "job-history",
            Title = "Job History",
            Endpoint = "api/JobHistory",
            IdField = "employeeId",
            Icon = "",
            Summary = "Career movement by employee and department.",
            ViewRoles = ["Admin", "HR", "Employee"],
            CreateRoles = ["Admin", "HR"],
            EditRoles = [],
            DeleteRoles = ["Admin"],
            Fields =
            [
                Select("employeeId", "Employee", "employees", required: true),
                Date("startDate", "Start date", required: true),
                Date("endDate", "End date", required: true),
                Select("jobId", "Job", "jobs", required: true, type: ApiFieldType.Text),
                Select("departmentId", "Department", "departments", required: true)
            ],
            Filters =
            [
                Filter("employee", "By employee", "api/JobHistory/{employeeId}", ["Admin", "HR", "Employee"], Select("employeeId", "Employee", "employees", required: true)),
                Filter("department", "By department", "api/JobHistory/department/{departmentId}", ["Admin", "HR", "Employee"], Select("departmentId", "Department", "departments", required: true))
            ]
        };
    }

    private static ApiResourceDefinition Roles()
    {
        return new ApiResourceDefinition
        {
            Key = "roles",
            Title = "Roles",
            Endpoint = "api/Roles",
            IdField = "roleId",
            Icon = "",
            Summary = "Role records used by authentication and authorization.",
            ViewRoles = ["Admin", "HR"],
            CreateRoles = ["Admin"],
            EditRoles = ["Admin"],
            DeleteRoles = ["Admin"],
            Fields =
            [
                Number("roleId", "Role", required: true, showInTable: false),
                Text("roleName", "Role name", required: true)
            ]
        };
    }

    private static ApiResourceDefinition Regions()
    {
        return new ApiResourceDefinition
        {
            Key = "regions",
            Title = "Regions",
            Endpoint = "api/Regions",
            IdField = "regionId",
            Icon = "",
            Summary = "Global regions and their countries.",
            ViewRoles = ["Admin", "HR", "Employee"],
            CreateRoles = ["Admin", "HR"],
            EditRoles = ["Admin", "HR"],
            DeleteRoles = ["Admin"],
            Fields =
            [
                Number("regionId", "Region", required: true, showInTable: false),
                Text("regionName", "Region name", required: true),
                Text("countryNames", "Countries", showInTable: true, readOnly: true)
            ]
        };
    }

    private static ApiResourceDefinition Countries()
    {
        return new ApiResourceDefinition
        {
            Key = "countries",
            Title = "Countries",
            Endpoint = "api/Countries",
            IdField = "countryId",
            Icon = "",
            Summary = "Countries grouped under regions.",
            ViewRoles = ["Admin", "HR"],
            CreateRoles = ["Admin", "HR"],
            EditRoles = ["Admin", "HR"],
            DeleteRoles = ["Admin"],
            Fields =
            [
                Text("countryId", "Country code", required: true, showInTable: false),
                Text("countryName", "Country name", required: true),
                Select("regionId", "Region", "regions", required: true),
                Text("regionName", "Region", showInTable: true, readOnly: true)
            ],
            Filters =
            [
                Filter("region", "By region", "api/Countries/region/{regionId}", ["Admin", "HR"], Select("regionId", "Region", "regions", required: true))
            ]
        };
    }

    private static ApiResourceDefinition Locations()
    {
        return new ApiResourceDefinition
        {
            Key = "locations",
            Title = "Locations",
            Endpoint = "api/Locations",
            IdField = "locationId",
            Icon = "",
            Summary = "Office locations by country.",
            ViewRoles = ["Admin", "HR", "Employee"],
            CreateRoles = ["Admin", "HR"],
            EditRoles = ["Admin", "HR"],
            DeleteRoles = ["Admin"],
            Fields =
            [
                Number("locationId", "Location", required: true, showInTable: false),
                Text("streetAddress", "Street address", required: true),
                Text("postalCode", "Postal code", required: true),
                Text("city", "City", required: true),
                Text("stateProvince", "State", required: true),
                Select("countryId", "Country", "countries", required: true, showInTable: false, type: ApiFieldType.Text),
                Text("countryName", "Country", showInTable: true, readOnly: true)
            ],
            Filters =
            [
                Filter("country", "By country", "api/Locations/country/{countryId}", ["Admin", "HR", "Employee"], Select("countryId", "Country", "countries", required: true, type: ApiFieldType.Text))
            ]
        };
    }

    private static ApiField Text(
        string name,
        string label,
        bool required = false,
        bool showInTable = true,
        bool readOnly = false,
        bool showInCreate = true,
        bool showInEdit = true,
        bool includeInCreatePayload = true,
        bool includeInEditPayload = true)
    {
        return Field(name, label, ApiFieldType.Text, required, showInTable, readOnly, showInCreate, showInEdit, includeInCreatePayload, includeInEditPayload);
    }

    private static ApiField Password(
        string name,
        string label,
        bool required = false,
        bool showInTable = false,
        bool showInEdit = true,
        bool includeInEditPayload = true)
    {
        return Field(
            name,
            label,
            ApiFieldType.Password,
            required,
            showInTable,
            showInEdit: showInEdit,
            includeInEditPayload: includeInEditPayload);
    }

    private static ApiField Number(
        string name,
        string label,
        bool required = false,
        bool showInTable = true,
        bool readOnly = false,
        bool showInCreate = true,
        bool showInEdit = true,
        bool includeInCreatePayload = true,
        bool includeInEditPayload = true)
    {
        return Field(name, label, ApiFieldType.Number, required, showInTable, readOnly, showInCreate, showInEdit, includeInCreatePayload, includeInEditPayload);
    }

    private static ApiField Date(
        string name,
        string label,
        bool required = false,
        bool showInCreate = true,
        bool includeInCreatePayload = true)
    {
        return Field(
            name,
            label,
            ApiFieldType.Date,
            required,
            showInCreate: showInCreate,
            includeInCreatePayload: includeInCreatePayload);
    }

    private static ApiField Select(
        string name,
        string label,
        string lookupKey,
        bool required = false,
        bool showInTable = true,
        bool readOnly = false,
        ApiFieldType type = ApiFieldType.Number)
    {
        var field = Field(name, label, type, required, showInTable, readOnly);
        field.LookupKey = lookupKey;
        return field;
    }

    private static ApiField Field(
        string name,
        string label,
        ApiFieldType type,
        bool required = false,
        bool showInTable = true,
        bool readOnly = false,
        bool showInCreate = true,
        bool showInEdit = true,
        bool includeInCreatePayload = true,
        bool includeInEditPayload = true)
    {
        return new ApiField
        {
            Name = name,
            Label = label,
            Type = type,
            Required = required,
            ShowInTable = showInTable,
            ReadOnly = readOnly,
            ShowInCreate = showInCreate,
            ShowInEdit = showInEdit,
            IncludeInCreatePayload = includeInCreatePayload,
            IncludeInEditPayload = includeInEditPayload
        };
    }

    private static ResourceFilter Filter(
        string key,
        string title,
        string endpointTemplate,
        string[] roles,
        params ApiField[] fields)
    {
        return new ResourceFilter
        {
            Key = key,
            Title = title,
            EndpointTemplate = endpointTemplate,
            Roles = roles,
            Fields = fields.ToList()
        };
    }
}
