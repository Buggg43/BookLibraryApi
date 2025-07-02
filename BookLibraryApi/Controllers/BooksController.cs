using BookLibraryApi.Data;
using BookLibraryApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BookLibraryApi.Models.Dtos;

namespace BookLibraryApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly LibraryDbContext _context;
        public BooksController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [Authorize(Roles="Admin")]
        [HttpGet("admin/all-books")]
        public async Task<IActionResult> GetAllBookFromAllUsers()
        {
            var books = await _context.Books.ToListAsync();
            return Ok(_mapper.Map<List<BookReadDto>>(books));
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var query = _context.Books.Where(b => b.UserId == userId);
            

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

            var booksDto = _mapper.Map<List<BookReadDto>>(books);

            var totalPages = (int)Math.Ceiling((double)totalItems/pageSize);

            return Ok(new
            {
                page,
                pageSize,
                totalItems,
                totalPages,
                data = booksDto
            });
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int id)
        {

            var chosen = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (chosen != null)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (chosen.Id != userId)
                    return Forbid();
                
                var dto = _mapper.Map<BookReadDto>(chosen);
                return Ok(dto);
            }
            else
                return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] BookCreateDto bookDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            

            var book = _mapper.Map<Book>(bookDto);
            book.UserId = userId;
            book.isRead = false;
            book.isFavorite = false;

            /*var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                Year = bookDto.Year,
                Description = bookDto.Description,
                isRead = false,
                isFavorite = false
            };*/
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody]BookUpdateDto book, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var found = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (found == null)
                return NotFound(id);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (found.UserId != userId)
                return Forbid();

            _mapper.Map(book, found);

            /* ręczne mapowanie // manual maping
            found.Title = book.Title;
            found.Author = book.Author;
            found.Year = book.Year;
            found.Description = book.Description;
            found.isRead = book.isRead;
            found.isFavorite = book.isFavorite;
            */

            await _context.SaveChangesAsync();

            return Ok(book);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var found = _context.Books.FirstOrDefault(b => b.Id == id);
            if (found == null)
                return NotFound(id);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (found.UserId != userId)
                return Forbid();
            
            _context.Books.Remove(found);
            await _context.SaveChangesAsync();

            return Ok($"Deleted {id}");
        }
    }

}
