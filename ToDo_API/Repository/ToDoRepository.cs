using ToDo_API.Data;
using ToDo_API.Models;

namespace ToDo_API.Repository
{
    public class ToDoRepository
    {
        private readonly DataContext context;

        public ToDoRepository(DataContext context) 
        {
            this.context = context;
        }

        public ICollection<ToDo> GetToDos()
        {
            return context.ToDos.OrderBy(t => t.Id).ToList();
        }
    }
}
