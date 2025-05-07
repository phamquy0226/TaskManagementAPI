namespace QuanLyCongViecAPI.Models
{
    public class WorkItem
    {
        public int WorkItemID { get; set; }
        public string TaskName { get; set; }
        public int Status { get; set; }
        public decimal Progress { get; set; }
        public string TaskType { get; set; }
        public bool IsPinned { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int AssignerID { get; set; }
        public string AssignerName { get; set; } // For display
        public int Priority { get; set; }
        public string DepartmentList { get; set; } // For display
        public string UserList { get; set; } // For display
    }
}