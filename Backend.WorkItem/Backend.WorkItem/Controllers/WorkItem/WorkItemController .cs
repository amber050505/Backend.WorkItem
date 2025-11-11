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
        public async Task<IActionResult> List()
        {
            var items = await _service.GetAllAsync();
            return View("List", items);
        }

        // New: GET /WorkItem
        [HttpGet()]
        public IActionResult New()
        {
            return View("New", new Model.WorkItem());
        }

        // Create: POST /WorkItem
        [HttpPost()]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Model.WorkItem model)
        {
            if (!ModelState.IsValid)
            {
                return View("New", model);
            }
            await _service.CreateAsync(model);
            TempData["SuccessMessage"] = "已送出新增請求（由背景服務寫入）";
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
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Model.WorkItem model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            model.Id = id;
            await _service.UpdateAsync(model);

            return Ok();
        }

        // Delete: Delete /WorkItem/{id}
        [HttpDelete("{id:int}")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
    }
}
