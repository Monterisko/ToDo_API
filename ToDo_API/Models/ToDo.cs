namespace ToDo_API.Models
{

    // Model Todo
    public class ToDo
    {
        public int Id { get; set; }

        public DateTime DateOfExpiry { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int percentageComplete { get; set; }

        public bool Done { get; set; } = false;
    }
}
