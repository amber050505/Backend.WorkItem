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
            return View(items);
        }

        // New: GET /WorkItem
        [HttpGet()]
        public IActionResult New()
        {
            return View(new Model.WorkItem());
        }

        // New: POST /WorkItem
        [HttpPost()]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> New([FromForm] Model.WorkItem model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await _service.CreateAsync(model);
            TempData["SuccessMessage"] = "已送出新增請求（由背景服務寫入）";
            return RedirectToAction(nameof(List));
        }

        // Edit: GET /WorkItem/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // Edit: PUT /WorkItem/{id}
        [HttpPut("{id:int}")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] Model.WorkItem model)
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
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
    }
}
