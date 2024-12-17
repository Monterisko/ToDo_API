using ToDo_API.Models;

namespace ToDo_API.Interfaces
{
    public interface IToDoRepository
    {
        ICollection<ToDo> GetToDos();
    }
}
