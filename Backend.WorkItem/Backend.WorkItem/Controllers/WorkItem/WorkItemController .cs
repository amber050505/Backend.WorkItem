using Backend.WorkItem.Service.WorkItem.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WorkItem.Controllers.WorkItem
{
    [Route("admin/work-items")]
    public class WorkItemController : Controller
    {
        private readonly IWorkItemService _service;
        public WorkItemController(IWorkItemService service) => _service = service;

        // List: GET /admin/work-items
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _service.GetAllAsync();
            return View("Index", items); // Views/Admin/WorkItems/Index.cshtml
        }

        // New form: GET /admin/work-items/new
        [HttpGet("new")]
        public IActionResult New()
        {
            return View("New", new Model.WorkItem ());
        }

        // Create: POST /admin/work-items/new
        [HttpPost("new")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Model.WorkItem model)
        {
            if (!ModelState.IsValid)
            {
                return View("New", model);
            }
            var newId = await _service.CreateAsync(model);
            TempData["SuccessMessage"] = "新增成功";
            return RedirectToAction("Index");
        }

        // Edit form: GET /admin/work-items/{id}/edit
        [HttpGet("{id:int}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View("Edit", item);
        }

        // Update: POST /admin/work-items/{id}/edit
        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Model.WorkItem model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }
            model.Id = id;
            var isUpdate = await _service.UpdateAsync(model);
            if (!isUpdate)
            {
                ModelState.AddModelError("", "更新失敗");
                return View("Edit", model);
            }
            TempData["SuccessMessage"] = "更新成功";
            return RedirectToAction("Index");
        }

        // Delete: POST /admin/work-items/{id}/delete
        [HttpPost("{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var isDelete = await _service.DeleteAsync(id);
            if (isDelete)
                TempData["SuccessMessage"] = "刪除成功";
            else
                TempData["ErrorMessage"] = "刪除失敗";
            return RedirectToAction("Index");
        }
    }
}
