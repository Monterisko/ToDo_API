using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDo_API.Conttrollers;
using ToDo_API.Data;
using ToDo_API.DTO;
using ToDo_API.Helper;
using ToDo_API.Interfaces;
using ToDo_API.Models;
using ToDo_API.Repository;

namespace Tests;

public class TodoControllerTest
{
    private readonly Mock<IToDoRepository> mockRepo;
    private readonly IMapper mapper;
    private readonly ToDoController controller;

    public TodoControllerTest()
    {
        mockRepo = new Mock<IToDoRepository>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ToDo, TodoDTO>();
            cfg.CreateMap<TodoDTO, ToDo>();
        });
        mapper = config.CreateMapper();
        controller = new ToDoController(mockRepo.Object, mapper);
    }

    [Fact]
    public void GetAll_ToDo_Success()
    {
        //Arrange

        var todos = new List<ToDo>
        {
            new ToDo { Id = 1, Title = "Task 1", Description = "Description 1" },
            new ToDo { Id = 2, Title = "Task 2", Description = "Description 2" }
        };
        mockRepo.Setup(repo => repo.GetTodos()).Returns(todos);

        //Act

        var result = controller.GetTodos();

        //Assert

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedTodos = Assert.IsType<List<TodoDTO>>(okResult.Value);
        Assert.Equal(2, returnedTodos.Count);
    }

    [Fact]
    public void GetAllTodos_ReturnsNotFound_WhenNoTodosExist()
    {
        // Arrange

        mockRepo.Setup(repo => repo.GetTodos()).Returns(new List<ToDo>());

        // Act

        var result = controller.GetTodos();

        // Assert

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No ToDos found.", notFoundResult.Value);
    }

    [Fact]
    public void GetTodoByID_ReturnsOkResult()
    {

        // Arrange

        var todoId = 1;
        var todo = new ToDo
        {
            Id = todoId,
            Title = "Task 1",
            Description = "Description 1",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };
        mockRepo.Setup(repo => repo.GetTodo(todoId)).Returns(todo);
        mockRepo.Setup(repo => repo.TodoExists(todoId)).Returns(true);

        // Act

        var result = controller.GetTodo(todoId);

        // Assert

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedTodo = Assert.IsType<ToDo>(okResult.Value);
        Assert.Equal(todoId, returnedTodo.Id);
    }

    [Fact]
    public void GetTodoByID_ReturnsNotFound_WhenTodoDoesNotExist()
    {

        // Arrange

        var todoId = 1;
        mockRepo.Setup(repo => repo.GetTodo(todoId)).Returns((ToDo)null);

        // Act

        var result = controller.GetTodo(todoId);

        // Assert

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"ToDo with ID {todoId} not found.", notFoundResult.Value);
    }

    [Fact]
    public void UpdateToDo_ReturnsNoContent()
    {

        // Arrange

        var todoId = 1;
        var existingTodo = new ToDo
        {
            Id = todoId,
            Title = "Task 1",
            Description = "Description 1",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };
        var updatedTodo = new TodoDTO
        {
            Id = todoId,
            Title = "Updated Title",
            Description = "Description 2",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };

        mockRepo.Setup(repo => repo.TodoExists(todoId)).Returns(true);
        mockRepo.Setup(repo => repo.GetTodo(todoId)).Returns(existingTodo);
        mockRepo.Setup(repo => repo.UpdateToDo(It.IsAny<ToDo>())).Returns(true);

        // Act

        var result = controller.UpdateTodo(todoId, updatedTodo);

        // Assert

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void UpdateToDo_ReturnsNotFound_WhenTodoDoesNotExists()
    {

        // Arrange

        var todoId = 1;
        var updatedTodo = new TodoDTO
        {
            Id = todoId,
            Title = "Updated Title",
            Description = "Description 2",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };

        mockRepo.Setup(repo => repo.TodoExists(todoId)).Returns(false);

        // Act

        var result = controller.UpdateTodo(todoId, updatedTodo);

        // Assert

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("ToDo not found.", notFoundResult.Value);
    }

    [Fact]
    public void UpdateToDo_ReturnsBadRequest_WhenIdMissmatch()
    {

        // Arrange

        var todoId = 1;
        var existingTodo = new ToDo
        {
            Id = todoId,
            Title = "Task 1",
            Description = "Description 1",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };
        var updatedTodo = new TodoDTO
        {
            Id = 2,
            Title = "Updated Title",
            Description = "Description 2",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };

        mockRepo.Setup(repo => repo.TodoExists(todoId)).Returns(true);

        // Act

        var result = controller.UpdateTodo(todoId, updatedTodo);

        // Assert

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ID missmatch.", badRequest.Value);
    }

    [Fact]
    public void UpdateToDo_ReturnsBadRequest_WhenHasBeenGivenNull()
    {

        // Arrange

        var todoId = 1;
        var existingTodo = new ToDo
        {
            Id = todoId,
            Title = "Task 1",
            Description = "Description 1",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };

        mockRepo.Setup(repo => repo.TodoExists(todoId)).Returns(true);

        // Act

        var result = controller.UpdateTodo(todoId, (TodoDTO)null);

        // Assert

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Updated ToDo cannot be null.", badRequest.Value);
    }

    [Fact]
    public void CreateToDo_ReturnsOk_WhenTodoDoesNotExits()
    {

        // Arrange

        var todoId = 1;
        var todo = new TodoDTO
        {
            Id = todoId,
            Title = "Updated Title",
            Description = "Description 2",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };

        mockRepo.Setup(repo => repo.TodoExists(todoId)).Returns(false);
        mockRepo.Setup(repo => repo.GetTodos()).Returns(new List<ToDo>());
        mockRepo.Setup(repo => repo.CreateToDo(It.IsAny<ToDo>())).Returns(true);

        // Act

        var result = controller.CreateTodo(todo);

        // Assert

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Successfully created", okResult.Value);
    }

    [Fact]
    public void CreateToDo_ReturnsBadRequest_WhenTodoExists()
    {

        // Arrange

        var todoId = 1;
        var existingTodo = new ToDo
        {
            Id = todoId,
            Title = "Updated Title",
            Description = "Description 2",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };
        var todo = new TodoDTO
        {
            Id = todoId,
            Title = "Updated Title",
            Description = "Description 2",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };
        var list = new List<ToDo>();
        list.Add(existingTodo);
        mockRepo.Setup(repo => repo.TodoExists(todoId)).Returns(true);
        mockRepo.Setup(repo => repo.GetTodos()).Returns(list);
        mockRepo.Setup(repo => repo.CreateToDo(It.IsAny<ToDo>())).Returns(false);

        // Act

        var result = controller.CreateTodo(todo);

        // Assert

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ToDo already exists", badRequest.Value);
    }

    [Fact]
    public void CreateToDo_ReturnsBadRequest_WhenHasBeenGivenNull()
    {

        // Arrange

        // Act

        var result = controller.CreateTodo((TodoDTO)null);

        // Assert

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Created ToDo cannot be null.", badRequest.Value);
    }

    [Fact]
    public void DeleteToDo_ReturnsNoContent_WhenTodoExists()
    {

        // Arrange

        var todoId = 1;
        var todo = new ToDo
        {
            Id = todoId,
            Title = "Updated Title",
            Description = "Description 2",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };

        mockRepo.Setup(repo => repo.TodoExists(todoId)).Returns(true);
        mockRepo.Setup(repo => repo.GetTodo(todoId)).Returns(todo);
        mockRepo.Setup(repo => repo.DeleteToDo(todo)).Returns(true);

        // Act

        var result = controller.DeleteTodo(todoId);

        // Assert

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void DeleteToDo_ReturnsNotFound_WhenTodoDoesNotExists()
    {

        // Arrange

        var todoId = 1;
        var todo = new ToDo
        {
            Id = todoId,
            Title = "Updated Title",
            Description = "Description 2",
            DateOfExpiry = DateTime.Now,
            percentageComplete = 50
        };

        mockRepo.Setup(repo => repo.TodoExists(todoId)).Returns(false);
        mockRepo.Setup(repo => repo.GetTodo(todoId)).Returns((ToDo)null);
        mockRepo.Setup(repo => repo.DeleteToDo(todo)).Returns(true);


        // Act

        var result = controller.DeleteTodo(todoId);

        // Assert

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Deleting a non-existent Todo", notFound.Value);
    }

    [Fact]
    public void GetTodayTodos_ReturnsNotFound_WhenAnyTodayTodoDoesNotExists()
    {
        //Arrange

        var todos = new List<ToDo>
        {
            
        };
        mockRepo.Setup(repo => repo.TodayTodoExists()).Returns(false);
        mockRepo.Setup(repo => repo.GetTodayTodos()).Returns(todos);

        //Act

        var result = controller.GetTodayTodo();

        //Assert

        Assert.IsType<NotFoundResult>(result);
        
    }

    [Fact]
    public void GetTomorrowTodos_ReturnsOk_WhenTodoExists()
    {
        //Arrange

        var todos = new List<ToDo>
        {
            new ToDo
            {
                Id = 1,
                Title = "Task 1",
                Description = "Description 1",
                DateOfExpiry = DateTime.Now.AddDays(1),
                percentageComplete = 50
            },
            new ToDo
            {
                Id = 2,
                Title = "Task 2",
                Description = "Description 2",
                DateOfExpiry = DateTime.Now.AddDays(1),
                percentageComplete = 40
            }
        };
        mockRepo.Setup(repo => repo.TomorrowTodoExists()).Returns(true);
        mockRepo.Setup(repo => repo.GetTomorrowTodos()).Returns(todos);

        //Act

        var result = controller.GetTomorrowTodo();

        //Assert

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedTodos = Assert.IsType<List<TodoDTO>>(okResult.Value);
        Assert.Equal(2, returnedTodos.Count);
    }

    [Fact]
    public void GetTomorrowTodos_ReturnsNotFound_WhenAnyTodayTodoDoesNotExists()
    {
        //Arrange

        var todos = new List<ToDo>
        {

        };
        mockRepo.Setup(repo => repo.TomorrowTodoExists()).Returns(false);
        mockRepo.Setup(repo => repo.GetTomorrowTodos()).Returns(todos);

        //Act

        var result = controller.GetTomorrowTodo();

        //Assert

        Assert.IsType<NotFoundResult>(result);

    }

    [Fact]
    public void GetTodayTodos_ReturnsOk_WhenTodoExists()
    {
        //Arrange

        var todos = new List<ToDo>
    {
        new ToDo
        {
            Id = 1,
            Title = "Task 1",
            Description = "Description 1",
            DateOfExpiry = DateTime.Now.AddHours(2),
            percentageComplete = 50
        },
        new ToDo
        {
            Id = 2,
            Title = "Task 2",
            Description = "Description 2",
            DateOfExpiry = DateTime.Now.AddHours(3),
            percentageComplete = 40
        }
    };
        mockRepo.Setup(repo => repo.TodayTodoExists()).Returns(true);
        mockRepo.Setup(repo => repo.GetTodayTodos()).Returns(todos);

        //Act

        var result = controller.GetTodayTodo();

        //Assert

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedTodos = Assert.IsType<List<TodoDTO>>(okResult.Value);
        Assert.Equal(2, returnedTodos.Count);
    }

    [Fact]
    public void GetWeeklyTodos_ReturnsOk_WhenTodoExists()
    {
        //Arrange

        var todos = new List<ToDo>
    {
        new ToDo
        {
            Id = 1,
            Title = "Task 1",
            Description = "Description 1",
            DateOfExpiry = DateTime.Now.AddDays(3),
            percentageComplete = 50
        },
        new ToDo
        {
            Id = 2,
            Title = "Task 2",
            Description = "Description 2",
            DateOfExpiry = DateTime.Now.AddDays(5),
            percentageComplete = 40
        }
    };
        mockRepo.Setup(repo => repo.WeeklyTodoExists()).Returns(true);
        mockRepo.Setup(repo => repo.GetWeeklyTodos()).Returns(todos);

        //Act

        var result = controller.GetWeeklyTodo();

        //Assert

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedTodos = Assert.IsType<List<TodoDTO>>(okResult.Value);
        Assert.Equal(2, returnedTodos.Count);
    }

    [Fact]
    public void GetWeeklyTodos_ReturnsNotFound_WhenAnyTodayTodoDoesNotExists()
    {
        //Arrange

        var todos = new List<ToDo>
        {

        };
        mockRepo.Setup(repo => repo.WeeklyTodoExists()).Returns(false);
        mockRepo.Setup(repo => repo.GetWeeklyTodos()).Returns(todos);

        //Act

        var result = controller.GetWeeklyTodo();

        //Assert

        Assert.IsType<NotFoundResult>(result);

    }
}

