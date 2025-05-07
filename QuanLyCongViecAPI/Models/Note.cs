namespace QuanLyCongViecAPI.Models
{
    public class Note
    {
        public int NoteID { get; set; }
        public int WorkItemID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}