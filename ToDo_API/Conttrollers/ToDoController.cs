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
            // Get all Todo from database
            var todos = mapper.Map<List<TodoDTO>>(todoRepository.GetTodos());
            // Check if any todo exists or the variable is not null
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
            // Checks whether the todo with the specified id exists
            if (!todoRepository.TodoExists(todoID))
                return NotFound($"ToDo with ID {todoID} not found.");
            // Get the todo from the database with the specified id 
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
            // Checks if there is a todo which ends today
            if (!todoRepository.TodayTodoExists())
                return NotFound();
            // Get the todo from the database which ends today
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
            // Checks if there is a todo which ends tomorrow
            if (!todoRepository.TomorrowTodoExists())
                return NotFound();
            // Get the todo from the database which ends tomorrow
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
            // Checks if there is a todo that ends in a week at most
            if (!todoRepository.WeeklyTodoExists())
                return NotFound();
            // Get the todo from the database which ends in a week at most
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
            // Checks if the given todo is not null
            if (todoCreate == null)
                return BadRequest("Created ToDo cannot be null.");
            // Checks that the specified todo title does not exist in the database
            var todo = todoRepository.GetTodos()
                .Where(t => t.Title.Trim().ToUpper() == todoCreate.Title.Trim().ToUpper()).FirstOrDefault();

            if (todo != null || todoRepository.TodoExists(todoCreate.Id))
            {
                return BadRequest("ToDo already exists");
            }
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            // Insert the todo into the database
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
            // Check if the given todo is not null
            if (updatedToDo == null)
                return BadRequest("Updated ToDo cannot be null.");
            // Checks if the id differs from the given todo
            if (todoID != updatedToDo.Id)
                return BadRequest("ID missmatch.");
            // Checks whether the todo with the specified id exists
            if (!todoRepository.TodoExists(todoID))
                return NotFound("ToDo not found.");
            if (!ModelState.IsValid)
                return BadRequest();
            // Get the todo from the database
            var todo = todoRepository.GetTodo(todoID);
            // Verifies that the todo was successfully retrieved from the database
            if (todo == null)
            {
                return BadRequest("Submitted data is invalid");
            }

            // if percentage of complete is 100 the todo is done
            if (updatedToDo.percentageComplete == 100)
            {
                todo.Done = true;
            }
            // update existing todo
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
            // Check if the given todo is not null
            if (updatedToDo == null)
                return BadRequest("Updated ToDo cannot be null.");

            // Checks if the id differs from the given todo
            if (todoID != updatedToDo.Id)
                return BadRequest("ID mismatch.");

            // Checks whether the todo with the specified id exists
            if (!todoRepository.TodoExists(todoID))
                return NotFound("ToDo not found.");
            if (!ModelState.IsValid)
                return BadRequest();

            // Get the todo from the database
            var todo = todoRepository.GetTodo(todoID);

            // Verifies that the todo was successfully retrieved from the database
            if (todo == null)
            {
                return BadRequest("Submitted data is invalid");
            }
            // if percentage of complete is 100 the todo is done
            if (updatedToDo.percentageComplete == 100)
            {
                todo.Done = true;
            }
            // Update pecentage complete of todo
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
            // Checks whether the todo with the specified id exists
            if (!todoRepository.TodoExists(todoID))
                return NotFound("Deleting a non-existent Todo");
            // Get the todo from the database
            var todoToDelete = todoRepository.GetTodo(todoID);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (todoRepository.GetTodo(todoID) != todoToDelete)
                return BadRequest("Deleting another Todo");
            // Delete todo from the database
            if(!todoRepository.DeleteToDo(todoToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting ToDo");
            }
            return NoContent();
        }
    }
}
