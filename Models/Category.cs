using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Slug { get; set; }

        public IList<Post> Posts { get; set; }
    }
}