namespace QuanLyCongViecAPI.Models
{
    public class NoteCreateModel
    {
        public int WorkItemID { get; set; }
        public string Content { get; set; }
        public DateTime DateCreate { get; set; } // Thêm DateCreate
    }
}
