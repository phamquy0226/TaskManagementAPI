using Microsoft.AspNetCore.Mvc;
using QuanLyCongViecAPI.Models;
using QuanLyCongViecAPI.Services;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    // Lấy danh sách user
    [HttpGet]
    public ActionResult<ResponseModel> GetUsers()
    {
        var result = _userService.GetUsers();
        return Ok(result);
    }

    // Thêm mới user
    [HttpPost]
    public ActionResult<ResponseModel> CreateUser([FromBody] string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return BadRequest("UserName cannot be empty.");
        }

        var result = _userService.InsertUser(userName);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    // Sửa thông tin user
    [HttpPut("{id}")]
    public ActionResult<ResponseModel> UpdateUser(int id, [FromBody] string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return BadRequest("UserName cannot be empty.");
        }

        var result = _userService.UpdateUser(id, userName);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    // Xoá user
    [HttpDelete("{id}")]
    public ActionResult<ResponseModel> DeleteUser(int id)
    {
        var result = _userService.DeleteUser(id);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
