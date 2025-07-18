using Microsoft.AspNetCore.Mvc;
using QuanLyCongViecAPI.Models;
using QuanLyCongViecAPI.Services;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly DepartmentService _departmentService;

    public DepartmentController(DepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

   
    [HttpGet]
    public ActionResult<ResponseModel> GetDepartments()
    {
        var result = _departmentService.GetDepartments();
        return Ok(result);
    }

    
    [HttpPost]
    public ActionResult<ResponseModel> CreateDepartment([FromBody] DepartmentCreateModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.DepartmentName))
        {
            return BadRequest("Department name is required.");
        }

        var result = _departmentService.CreateDepartment(model);
        if (result.Success)
        {
            return CreatedAtAction(nameof(GetDepartments), new { id = result.Data }, result);
        }
        return BadRequest(result);
    }

    
    [HttpPut("{id}")]
    public ActionResult<ResponseModel> UpdateDepartment(int id, [FromBody] DepartmentUpdateModel model)
    {
        if (model == null || id != model.DepartmentID)
        {
            return BadRequest("Invalid department data.");
        }

        var result = _departmentService.UpdateDepartment(model);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

   
    [HttpDelete("{id}")]
    public ActionResult<ResponseModel> DeleteDepartment(int id)
    {
        var result = _departmentService.DeleteDepartment(id);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
