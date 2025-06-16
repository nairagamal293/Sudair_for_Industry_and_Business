using System.ComponentModel.DataAnnotations;

namespace Factory.Models
{
    public class QuoteRequest
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Company { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        public string Address { get; set; }

        // Product details (simplified from your form)
        public string ProductDetails { get; set; }
        public string Quantity { get; set; }
        public string IntendedUse { get; set; }
        public string Budget { get; set; }
        public string DeliveryTimeline { get; set; }
        public string AdditionalNotes { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
