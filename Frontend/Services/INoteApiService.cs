using Frontend.Models;

namespace Frontend.Services
{
    using Frontend.Models;

    public interface INoteApiService
    {
        Task<List<NoteModel>> GetNotesByWorkItemIdAsync(int workItemId);
        Task<bool> AddNoteAsync(int workItemId, string content);  
        Task<bool> DeleteNoteAsync(int workItemId, int noteId);
    }


}
