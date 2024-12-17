using ToDo_API.Models;

namespace ToDo_API.Interfaces
{
    public interface IToDoRepository
    {
        // Get all ToDo from database
        ICollection<ToDo> GetTodos();

        // Get from database ToDo with specific ID
        ToDo GetTodo(int id);

        // Check if ToDo exists
        bool TodoExists(int todoID);



    }
}
