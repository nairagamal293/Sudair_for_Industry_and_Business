using System.ComponentModel.DataAnnotations;

namespace Factory.Models
{
    public class AdminUser
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string? Token { get; set; }  // Changed to nullable
        public DateTime? TokenExpiry { get; set; }
    }
}
