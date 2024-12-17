using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using ToDo_API.DTO;
using ToDo_API.Interfaces;
using ToDo_API.Models;

namespace ToDo_API.Conttrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : Controller
    {
        private readonly IToDoRepository todoRepository;
        private readonly IMapper mapper;

        public ToDoController(IToDoRepository todoRepository, IMapper mapper)
        {
            this.todoRepository = todoRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ToDo>))]

        public IActionResult GetTodos()
        {
            var todos = mapper.Map<List<TodoDTO>>(todoRepository.GetTodos());
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(todos);
        }

        [HttpGet("{todoID}")]
        [ProducesResponseType(200, Type = typeof(ToDo))]
        [ProducesResponseType(400)]
        public IActionResult GetTodo(int todoID)
        {
            if(!todoRepository.TodoExists(todoID))
                return NotFound();

            var todo = mapper.Map<List<TodoDTO>>(todoRepository.GetTodos());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(todo);
        }

    }
}
