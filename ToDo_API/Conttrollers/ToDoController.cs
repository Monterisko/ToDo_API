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

            if (todos == null || !todos.Any()) 
            { 
                return NotFound("No ToDos found.");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(todos);
        }

        [HttpGet("{todoID}")]
        [ProducesResponseType(200, Type = typeof(ToDo))]
        [ProducesResponseType(400)]
        public IActionResult GetTodo(int todoID)
        {
            if(!todoRepository.TodoExists(todoID))
                return NotFound($"ToDo with ID {todoID} not found.");

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
            var todo = mapper.Map<List<TodoDTO>>(todoRepository.GetTodayTodos());

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
                return BadRequest("Created ToDo cannot be null.");
            var todo = todoRepository.GetTodos()
                .Where(t => t.Title.Trim().ToUpper() == todoCreate.Title.Trim().ToUpper()).FirstOrDefault();

            if (todo != null || todoRepository.TodoExists(todoCreate.Id))
            {
                return BadRequest("ToDo already exists");
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
                return BadRequest("Updated ToDo cannot be null.");
            if (todoID != updatedToDo.Id)
                return BadRequest("ID missmatch.");
            if (!todoRepository.TodoExists(todoID))
                return NotFound("ToDo not found.");
            if (!ModelState.IsValid)
                return BadRequest();
            var todo = todoRepository.GetTodo(todoID);
            if(updatedToDo.percentageComplete == 100)
            {
                todo.Done = true;
            }
            if (todo == null)
            {
                return BadRequest("Submitted data is invalid");
            }
            mapper.Map(updatedToDo, todo);
            if (!todoRepository.UpdateToDo(todo))
            {
                ModelState.AddModelError("", "Something went wrong updating ToDo");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpPut("{todoID}/percentage")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdatePercentageTodo(int todoID, [FromBody] UpdatePercentageCompleteDTO updatedToDo)
        {
            if (updatedToDo == null)
                return BadRequest("Updated ToDo cannot be null.");
            if (todoID != updatedToDo.Id)
                return BadRequest("ID mismatch.");
            if (!todoRepository.TodoExists(todoID))
                return NotFound("ToDo not found.");
            if (!ModelState.IsValid)
                return BadRequest();
            var todo = todoRepository.GetTodo(todoID);
            if (todo == null)
            {
                return BadRequest("Submitted data is invalid");
            }
            if (updatedToDo.percentageComplete == 100)
            {
                todo.Done = true;
            }
            if (updatedToDo.percentageComplete >= 0) { todo.percentageComplete = updatedToDo.percentageComplete; }
            if (!todoRepository.UpdateToDo(todo))
            {
                ModelState.AddModelError("", "Something went wrong updating ToDo");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{todoID}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTodo(int todoID)
        {
            if (!todoRepository.TodoExists(todoID))
                return NotFound("Deleting a non-existent Todo");
            var todoToDelete = todoRepository.GetTodo(todoID);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (todoRepository.GetTodo(todoID) != todoToDelete)
                return BadRequest("Deleting another Todo");
            if(!todoRepository.DeleteToDo(todoToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting ToDo");
            }
            return NoContent();
        }
    }
}
