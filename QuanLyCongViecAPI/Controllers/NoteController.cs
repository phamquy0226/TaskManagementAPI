using Microsoft.AspNetCore.Mvc;
using QuanLyCongViecAPI.Models;
using QuanLyCongViecAPI.Services;

[ApiController]
[Route("api/workitems/{workItemId}/notes")]
public class NoteController : ControllerBase
{
    private readonly NoteService _noteService;

    public NoteController(NoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public ActionResult<ResponseModel> GetNoteList(int workItemId)
    {
        var result = _noteService.GetNotesByWorkItem(workItemId);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<ResponseModel> AddNote(int workItemId, NoteCreateModel model)
    {
        if (!ModelState.IsValid || workItemId != model.WorkItemID)
        {
            return BadRequest(ModelState);
        }
        var result = _noteService.AddNote(model);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut("{id}")]
    public ActionResult<ResponseModel> UpdateNote(int id, NoteUpdateModel model)
    {
        if (!ModelState.IsValid || id != model.NoteID)
        {
            return BadRequest(ModelState);
        }
        var result = _noteService.UpdateNote(model);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    public ActionResult<ResponseModel> DeleteNote(int id)
    {
        var result = _noteService.DeleteNote(id);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}