using Factory.Models;
using Factory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System;
using Factory.Data;
using Microsoft.EntityFrameworkCore;
using Factory.DTOs;

namespace Factory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly FileStorageService _fileService;

        public ProductsController(AppDbContext context, FileStorageService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // Add to your GetProducts endpoint in ProductsController
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? categoryId = null)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ImagePath = p.ImagePath,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ImagePath = p.ImagePath,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromForm] ProductCreateModel model)
        {
            if (model.ImageFile == null || model.ImageFile.Length == 0)
            {
                return BadRequest("Product image is required");
            }

            var imagePath = await _fileService.SaveFile(model.ImageFile);
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                ImagePath = imagePath,
                CategoryId = model.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productDto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImagePath = product.ImagePath,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateModel model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            product.Name = model.Name;
            product.Description = model.Description;
            product.CategoryId = model.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            // Only update the image if a new file is provided
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                // Delete old image
                _fileService.DeleteFile(product.ImagePath);

                // Save new image
                product.ImagePath = await _fileService.SaveFile(model.ImageFile);
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/products/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _fileService.DeleteFile(product.ImagePath);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class ProductCreateModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }
    }

    public class ProductUpdateModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; } // Now nullable
    }
}
