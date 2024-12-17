using Microsoft.AspNetCore.Mvc;
using System.Collections;
using ToDo_API.Interfaces;
using ToDo_API.Models;

namespace ToDo_API.Conttrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : Controller
    {
        private readonly IToDoRepository todoRepository;

        public ToDoController(IToDoRepository todoRepository)
        {
            this.todoRepository = todoRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ToDo>))]

        public IActionResult GetTodos()
        {
            var todos = todoRepository.GetTodos();
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(todos);
        }
    }
}
