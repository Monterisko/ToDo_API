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

        public ICollection<ToDo> GetTodos()
        {
            return _context.Todos.OrderBy(t => t.Id).ToList();
        }
    }
}
