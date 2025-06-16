using BookLibraryApi.Data;
using BookLibraryApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookLibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        public BooksController(LibraryDbContext context)
        {
            _context=context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var books = _context.Books;

            return Ok(books);
        }
        [HttpGet]
        [Route("/{Id}")]
        public IActionResult GetById([FromQuery]int id)
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
        public IActionResult AddBook([FromBody]Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();


            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

    }
}
