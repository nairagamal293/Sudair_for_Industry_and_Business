using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

public class QuoteRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100)]
    public string FullName { get; set; }

    [StringLength(100)]
    public string Company { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [StringLength(20)]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Country is required")]
    [StringLength(50)]
    public string Country { get; set; }

    [Required(ErrorMessage = "City is required")]
    [StringLength(50)]
    public string City { get; set; }

    [StringLength(200)]
    public string Address { get; set; }

    // Store as JSON string
    public string ProductDetails { get; set; }

    // For simple display
    [NotMapped]
    public string ProductNames
    {
        get
        {
            try
            {
                var products = JsonSerializer.Deserialize<List<QuoteProduct>>(ProductDetails);
                return string.Join(", ", products.Select(p => p.Name));
            }
            catch
            {
                return "Custom Product";
            }
        }
    }

    public string Quantity { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}

public class QuoteProduct
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public string Notes { get; set; }
    // Add any other product-related fields you need
}