namespace HumanResource.API.DTOs
{
    public class JobHistoryDto
    {
        public int EmployeeId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string JobId { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
    }
}
