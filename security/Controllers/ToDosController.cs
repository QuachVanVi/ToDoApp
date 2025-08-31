using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using security.Data;
using security.Models;
using security.ViewModels;
using Ganss.Xss;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace security.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class TodosController(AppDbContext context) : ControllerBase
    {

        private readonly AppDbContext _context = context;
        private readonly HtmlSanitizer _htmlSanitizer = new();


    
        [HttpDelete("clearAll")]
        public async Task<IActionResult> DeleteAllTodos()
        {
            var todos = await _context.Todos.ToListAsync();
            if (!todos.Any()) return NotFound(new { success = false, error = "No todos found" });

            _context.Todos.RemoveRange(todos);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }


       
        [HttpPost]
        public async Task<IActionResult> AddTodo([FromBody] ToDoPostViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            model.Title = _htmlSanitizer.Sanitize(model.Title);

            var todo = new Todo { Title = model.Title };
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, new { success = true, data = todo });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var todos = await _context.Todos.ToListAsync();
            return Ok(new { success = true, data = todos });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodo(string id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) return NotFound(new { success = false, error = "Todo not found" });

            return Ok(new { success = true, data = todo });
        }

        

    }
}
