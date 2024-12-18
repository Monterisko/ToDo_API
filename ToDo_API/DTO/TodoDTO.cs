namespace ToDo_API.DTO
{
    public class TodoDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; } 

        public DateTime DateOfExpiry { get; set; }

        public int percentageComplete { get; set; }
    }
}
