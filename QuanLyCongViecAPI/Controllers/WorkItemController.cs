using Microsoft.AspNetCore.Mvc;
using QuanLyCongViecAPI.Models;
using QuanLyCongViecAPI.Services;
using System;

[ApiController]
[Route("api/[controller]")]
public class WorkItemController : ControllerBase
{
    private readonly WorkItemService _workItemService;

    public WorkItemController(WorkItemService workItemService)
    {
        _workItemService = workItemService;
    }

    [HttpGet]
    public ActionResult<ResponseModel> GetWorkItemList(
    [FromQuery] string? userName,
    [FromQuery] string? assignerName,
    [FromQuery] DateTime? startDateFrom,
    [FromQuery] DateTime? startDateTo,
    [FromQuery] DateTime? endDateFrom,
    [FromQuery] DateTime? endDateTo,
    [FromQuery] int? status,
    [FromQuery] int? departmentID,
    [FromQuery] string? searchTaskName,
    [FromQuery] int? priority,
    [FromQuery] bool? isPinned,
    [FromQuery] int pageNumber = 1,       // Thêm tham số phân trang
    [FromQuery] int pageSize = 20)        // Thêm tham số phân trang
    {
        var result = _workItemService.GetWorkItemList(
            userName, assignerName, startDateFrom, startDateTo, endDateFrom, endDateTo,
            status, departmentID, searchTaskName, priority, isPinned,
            pageNumber, pageSize);
        return Ok(result);
    }


    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var result = _workItemService.GetWorkItemById(id);
        return result.Success ? Ok(result) : NotFound(result);
    }


    [HttpPost]
    public ActionResult<ResponseModel> CreateWorkItem(WorkItemCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = _workItemService.CreateWorkItem(model);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateWorkItem(int id, [FromBody] WorkItemUpdateModel model)
    {
        if (id != model.WorkItemID)
        {
            return BadRequest("WorkItemID mismatch.");
        }

        var result = _workItemService.UpdateWorkItem(model);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteWorkItem(int id)
    {
        var result = _workItemService.DeleteWorkItem(id);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }


}