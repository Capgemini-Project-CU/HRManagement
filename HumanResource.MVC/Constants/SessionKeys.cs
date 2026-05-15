namespace HumanResource.MVC.Constants;


// Centralises all HttpContext.Session key names so magic strings are never
// scattered across controllers.

public static class SessionKeys
{
    public const string JwtToken         = "JwtToken";
    public const string UserEmail        = "UserEmail";
    public const string UserRole         = "UserRole";
    public const string TokenExpiration  = "TokenExpiration";
    public const string EmployeeId       = "EmployeeId";
}
