using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
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

            var todo = mapper.Map<ToDo>(todoRepository.GetTodo(todoID));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(todo);
        }

        [HttpGet("today")]
        [ProducesResponseType(200, Type = typeof(ToDo))]
        [ProducesResponseType(400)]
        public IActionResult GetTodayTodo()
        {
            if (!todoRepository.TodayTodoExists())
                return NotFound();
            var todo = mapper.Map <List<TodoDTO>>(todoRepository.GetTodayTodos());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(todo);
        }

        [HttpGet("tomorrow")]
        [ProducesResponseType(200, Type = typeof(ToDo))]
        [ProducesResponseType(400)]
        public IActionResult GetTomorrowTodo()
        {
            if (!todoRepository.TomorrowTodoExists())
                return NotFound();
            var todo = mapper.Map<List<TodoDTO>>(todoRepository.GetTomorrowTodos());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(todo);
        }

        [HttpGet("weekly")]
        [ProducesResponseType(200, Type = typeof(ToDo))]
        [ProducesResponseType(400)]
        public IActionResult GetWeeklyTodo()
        {
            if (!todoRepository.WeeklyTodoExists())
                return NotFound();
            var todo = mapper.Map<List<TodoDTO>>(todoRepository.GetWeeklyTodos());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(todo);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTodo([FromBody]TodoDTO todoCreate)
        {
            if (todoCreate == null)
                return BadRequest(ModelState);
            var todo = todoRepository.GetTodos()
                .Where(t => t.Title.Trim().ToUpper() == todoCreate.Title.Trim().ToUpper()).FirstOrDefault();

            if (todo != null)
            {
                ModelState.AddModelError("", "ToDo already exists");
                return StatusCode(422, ModelState);
            }
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var todoMap = mapper.Map<ToDo>(todoCreate);
            if(!todoRepository.CreateToDo(todoMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpPut("{todoID}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateTodo(int todoID, [FromBody]TodoDTO updatedToDo)
        {
            if (updatedToDo == null)
                return BadRequest(ModelState);
            if (todoID != updatedToDo.Id)
                return BadRequest(ModelState);
            if (!todoRepository.TodoExists(todoID))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest();
            var existingTodo = todoRepository.GetTodo(todoID);
            if (updatedToDo.Title != null) { existingTodo.Title = updatedToDo.Title; }
            if (updatedToDo.Description != null) { existingTodo.Description = updatedToDo.Description; }
            if (updatedToDo.DateOfExpiry != DateTime.MinValue) { existingTodo.DateOfExpiry = updatedToDo.DateOfExpiry; }
            if (updatedToDo.percentageComplete >= 0) { existingTodo.percentageComplete = updatedToDo.percentageComplete; }

            if (!todoRepository.UpdateToDo(existingTodo))
            {
                ModelState.AddModelError("", "Something went wrong updating ToDo");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


    
    }
}
