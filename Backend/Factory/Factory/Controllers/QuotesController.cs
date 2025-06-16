using Factory.Data;
using Factory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Factory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuotesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/quotes
        [HttpPost]
        public async Task<IActionResult> SubmitQuote([FromBody] QuoteRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.QuoteRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Quote request submitted successfully" });
        }

        // GET: api/quotes
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetQuoteRequests()
        {
            var quotes = await _context.QuoteRequests
                .OrderByDescending(q => q.SubmittedAt)
                .ToListAsync();

            return Ok(quotes);
        }

        // GET: api/quotes/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetQuoteRequest(int id)
        {
            var quote = await _context.QuoteRequests.FindAsync(id);
            if (quote == null)
                return NotFound();

            quote.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(quote);
        }
    }
}
