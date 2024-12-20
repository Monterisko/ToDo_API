using System.ComponentModel.DataAnnotations;

namespace ToDo_API.DTO
{
    public class TodoDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public DateTime DateOfExpiry { get; set; }

        [Required]
        public int percentageComplete { get; set; }

        public bool Done { get; set; } = false;

    }

    public class UpdatePercentageCompleteDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int percentageComplete { get; set; }
    }
}
