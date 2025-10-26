using Backend.WorkItem.Service.WorkItem.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WorkItem.Controllers.WorkItem
{
    [Route("[controller]")]
    public class WorkItemController : Controller
    {
        private readonly IWorkItemService _service;
        public WorkItemController(IWorkItemService service) => _service = service;

        // List: GET /WorkItem/List
        [HttpGet("List")]
        public async Task<IActionResult> Index()
        {
            var items = await _service.GetAllAsync();
            return View("List", items);
        }

        // New: GET /WorkItem
        [HttpGet()]
        public IActionResult New()
        {
            return View("New", new Model.WorkItem ());
        }

        // Create: POST /WorkItem
        [HttpPost()]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Model.WorkItem model)
        {
            if (!ModelState.IsValid)
            {
                return View("New", model);
            }
            var newId = await _service.CreateAsync(model);
            TempData["SuccessMessage"] = "新增成功";
            return RedirectToAction("List");
        }

        // Edit: GET /WorkItem/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View("Edit", item);
        }

        // Update: PUT /WorkItem/{id}
        [HttpPut("{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Model.WorkItem model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.Id = id;
            var isUpdate = await _service.UpdateAsync(model);
            if (!isUpdate)
                return BadRequest("更新失敗");

            return Ok();
        }

        // Delete: Delete /WorkItem/{id}
        [HttpDelete("{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var isDelete = await _service.DeleteAsync(id);
            if (!isDelete)
                return BadRequest("刪除失敗");

            return Ok();
        }
    }
}
