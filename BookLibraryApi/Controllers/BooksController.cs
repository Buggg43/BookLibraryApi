using BookLibraryApi.Data;
using BookLibraryApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookLibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? author, 
            [FromQuery] bool? isRead, 
            [FromQuery] bool? isFavorite, 
            [FromQuery] DateTime? year, 
            [FromQuery] int page = 1 , 
            [FromQuery] int pageSize = 10)
        {
            throw new Exception("Coś poszło bardzo źle");


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var query = _context.Books.AsQueryable();
            

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(a => a.Author == author);
            }
            if(year.HasValue)
            {
                query = query.Where(b => b.Year == year);
            }
            if (isRead.HasValue)
            {
                query = query.Where(b => b.isRead == isRead.Value);
            }
            if (isFavorite.HasValue)
            {
                query = query.Where(b => b.isFavorite == isFavorite.Value);
            }

            var totalItems = await query.CountAsync();

            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            var totalPages = (int)Math.Ceiling((double)totalItems/pageSize);

            return Ok(new
            {
                page,
                pageSize,
                totalItems,
                totalPages,
                data = books
            });
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int id)
        {

            var chosen = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (chosen != null)
            {
                return Ok(chosen);
            }
            else
                return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody]Book book, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var found = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (found == null)
                return NotFound(id);


            found.Title = book.Title;
            found.Author = book.Author;
            found.Year = book.Year;
            found.Description = book.Description;
            found.isRead = book.isRead;
            found.isFavorite = book.isFavorite;

            await _context.SaveChangesAsync();

            return Ok(found);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var found = _context.Books.FirstOrDefault(b => b.Id == id);
            if (found == null)
                return NotFound(id);
            
            _context.Books.Remove(found);
            await _context.SaveChangesAsync();

            return Ok($"Deleted {id}");
        }
    }

}
