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

        // Get Incoming ToDo for today
        public ICollection<ToDo> GetTodayTodos()
        {
            return _context.Todos.Where(t => (t.DateOfExpiry.Day == DateTime.Today.Day && t.DateOfExpiry.Month == DateTime.Today.Month && t.DateOfExpiry.Year == DateTime.Today.Year)).ToList();
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

        // Get Incoming ToDo for next day
        public ICollection<ToDo> GetTomorrowTodos()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            return _context.Todos.Where(t => (t.DateOfExpiry.Day == tomorrow.Day && t.DateOfExpiry.Month == tomorrow.Month && t.DateOfExpiry.Year == tomorrow.Year)).ToList();
        }

        // Get Incoming ToDo for current week
        public ICollection<ToDo> GetWeeklyTodos()
        {
            var week = DateTime.Today.AddDays(7);
            var today = DateTime.Today;
            return _context.Todos.Where(t => ((t.DateOfExpiry.Day <= week.Day && t.DateOfExpiry.Day >= today.Day) && (t.DateOfExpiry.Month == week.Month && t.DateOfExpiry.Month >= today.Month) && (t.DateOfExpiry.Year <= week.Year && t.DateOfExpiry.Year >= today.Year))).ToList();
        }

        // Check if ToDo exists
        public bool TodoExists(int todoID)
        {
            return _context.Todos.Any(t => t.Id == todoID);
        }

        // Check if ToDo for today exists
        public bool TodayTodoExists()
        {
            return _context.Todos.Any(t => (t.DateOfExpiry.Day == DateTime.Today.Day && t.DateOfExpiry.Month == DateTime.Today.Month && t.DateOfExpiry.Year == DateTime.Today.Year));
        }

        // Check if ToDo for next day exists
        public bool TomorrowTodoExists()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            return _context.Todos.Any(t => (t.DateOfExpiry.Day == tomorrow.Day && t.DateOfExpiry.Month == tomorrow.Month && t.DateOfExpiry.Year == tomorrow.Year));
        }

        // Check if ToDo for week exists
        public bool WeeklyTodoExists()
        {
            var week = DateTime.Today.AddDays(7);
            var today = DateTime.Today;
            return _context.Todos.Any(t => ((t.DateOfExpiry.Day <= week.Day && t.DateOfExpiry.Day >= today.Day) && (t.DateOfExpiry.Month == week.Month && t.DateOfExpiry.Month >= today.Month) && (t.DateOfExpiry.Year <= week.Year && t.DateOfExpiry.Year >= today.Year)));
        }

        // Add todo to the database
        public bool CreateToDo(ToDo toDo)
        {
            _context.Add(toDo);
            return Save();
        }
        // Save changes in the database
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        // Update the todo in the database
        public bool UpdateToDo(ToDo todo)
        {
            _context.Update(todo);
            return Save();
        }
        // Delete the todo from the database
        public bool DeleteToDo(ToDo todo)
        {
            _context.Remove(todo);
            return Save();
        }
    }
}
