using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Domain.Entities
{
    public class Tag
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int Count { get; set; }

    }
}
