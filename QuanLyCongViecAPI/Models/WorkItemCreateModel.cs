namespace QuanLyCongViecAPI.Models
{
    public class WorkItemCreateModel
    {
        public string TaskName { get; set; }
        public int Status { get; set; }
        public decimal Progress { get; set; }
        public string TaskType { get; set; }
        public bool IsPinned { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int AssignerID { get; set; }
        public int Priority { get; set; }
        public List<int> DepartmentIDs { get; set; }
        public List<int> UserIDs { get; set; }
    }
}