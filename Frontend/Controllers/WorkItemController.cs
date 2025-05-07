using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Frontend.Controllers
{
    public class WorkItemController : Controller
    {
        private readonly IWorkItemApiService _workItemApiService;
        private readonly IDepartmentApiService _departmentApiService;
        private readonly IUserApiService _userApiService;
        private readonly INoteApiService _noteApiService;

        public WorkItemController(IWorkItemApiService workItemApiService, IDepartmentApiService departmentApiService, IUserApiService userApiService, INoteApiService noteApiService)
        {
            _workItemApiService = workItemApiService;
            _departmentApiService = departmentApiService;
            _userApiService = userApiService;
            _noteApiService = noteApiService;
        }


        public async Task<IActionResult> Index([FromQuery] WorkItemFilterModel filter)
        {
            var workItems = await _workItemApiService.GetFilteredAsync(filter);
            var users = await _userApiService.GetAllAsync(); 
            var departments = await _departmentApiService.GetAllAsync(); 

            ViewBag.Users = users;
            ViewBag.Departments = departments;
            return View(workItems);
        }



        public async Task<IActionResult> Create()
        {
            var users = await _userApiService.GetAllAsync();
            var departments = await _departmentApiService.GetAllAsync();

            ViewBag.Users = users;
            ViewBag.Departments = departments;

            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(WorkItemCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _workItemApiService.CreateAsync(model);
            if (result)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Không thể tạo công việc mới.");
            return View(model);
        }


        public async Task<IActionResult> Detail(int id)
        {
            var workItem = await _workItemApiService.GetWorkItemDetailAsync(id);
            if (workItem == null) return NotFound();

            workItem.Notes = await _noteApiService.GetNotesByWorkItemIdAsync(id);
            return View(workItem);
        }

        [HttpPost]
        public async Task<IActionResult> AddNote(int workItemId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                ModelState.AddModelError("", "Nội dung ghi chú không thể để trống.");
                return RedirectToAction("Details", new { id = workItemId });
            }

            // Gọi phương thức AddNoteAsync để thêm ghi chú
            var result = await _noteApiService.AddNoteAsync(workItemId, content);

            if (result)
            {
                return RedirectToAction("Detail", new { id = workItemId });
            }
            else
            {
                ModelState.AddModelError("", "Có lỗi khi thêm ghi chú.");
                return RedirectToAction("Detail", new { id = workItemId });
            }
        }



        [HttpPost]
        public async Task<IActionResult> DeleteNote(int workItemId, int noteId)
        {
            // Xoá ghi chú cho công việc
            await _noteApiService.DeleteNoteAsync(workItemId, noteId);
            return RedirectToAction("Detail", new { id = workItemId });
        }

    }
}
