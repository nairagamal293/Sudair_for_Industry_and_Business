using System.ComponentModel.DataAnnotations;

namespace Factory.DTOs
{
    // CategoryDTO.cs
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // CategoryCreateDTO.cs
    public class CategoryCreateDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }
    }

    // CategoryUpdateDTO.cs
    public class CategoryUpdateDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
