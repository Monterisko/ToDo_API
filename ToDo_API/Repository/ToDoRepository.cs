using ToDo_API.Data;
using ToDo_API.Interfaces;
using ToDo_API.Models;

namespace ToDo_API.Repository
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly DataContext _context;

        public ToDoRepository(DataContext context) 
        {
            _context = context;
        }

        // Get from database ToDo with specific ID
        public ToDo GetTodo(int id)
        {
            return _context.Todos.Where(t => t.Id == id).First();
        }


        // Get all ToDo from database
        public ICollection<ToDo> GetTodos()
        {
            return _context.Todos.OrderBy(t => t.Id).ToList();
        }

        // Check if ToDo exists
        public bool TodoExists(int todoID)
        {
            return _context.Todos.Any(t => t.Id == todoID);
        }
    }
}
