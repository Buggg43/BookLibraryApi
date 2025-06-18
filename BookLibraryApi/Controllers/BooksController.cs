using BookLibraryApi.Data;
using BookLibraryApi.Models;
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
        public IActionResult GetAll()
        {
            var books = _context.Books;

            return Ok(books);
        }
        [HttpGet("{Id}")]
        public IActionResult GetById(int id)
        {

            var chosen = _context.Books.FirstOrDefault(b => b.Id == id);
            if (chosen != null)
            {
                return Ok(chosen);
            }
            else
                return NotFound();
        }
        [HttpPost]
        public IActionResult AddBook([FromBody] Book book)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Books.Add(book);
            _context.SaveChanges();


            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }
        [HttpPut("{id}")]
        public IActionResult Update([FromBody]Book book, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var found = _context.Books.FirstOrDefault(b => b.Id == id);
            if (found == null)
                return NotFound(id);


            found.Title = book.Title;
            found.Author = book.Author;
            found.Year = book.Year;
            found.Description = book.Description;
            found.isRead = book.isRead;
            found.isFavorite = book.isFavorite;
            _context.SaveChanges();
            return Ok(found);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var found = _context.Books.FirstOrDefault(b => b.Id == id);
            if (found == null)
                return NotFound(id);
            
            _context.Books.Remove(found);
            _context.SaveChanges();

            return Ok($"Deleted {id}");
        }
    }

}
