namespace ToDo_API.Models
{

    //
    public class ToDo
    {
        public int Id { get; set; }

        public DateTime DateOfExpiry { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int percentageComplete { get; set; }
    }
}
