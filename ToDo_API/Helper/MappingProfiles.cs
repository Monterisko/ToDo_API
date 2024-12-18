using AutoMapper;
using ToDo_API.DTO;
using ToDo_API.Models;

namespace ToDo_API.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<ToDo, TodoDTO>();
            CreateMap<TodoDTO, ToDo>();
        }
    }
}
