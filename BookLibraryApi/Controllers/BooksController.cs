using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Features.Users.Queries;
using MediatR;
using BookLibraryApi.Features.Users.Commands;

namespace BookLibraryApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [Authorize(Roles="Admin")]
        [HttpGet("admin/all-books")]
        public async Task<IActionResult> GetAllBookFromAllUsers()
        {
            return (IActionResult)await _mediator.Send(new GetAllBooksQuery()); ;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetUserBooksDto filters)
        {
            return (IActionResult)await _mediator.Send(new GetUserBooksQuery(filters, User)); ;
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return (IActionResult)await _mediator.Send(new GetBookByIdQuery(id, User)); ;
        }
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] BookCreateDto bookDto)
        {
            return (IActionResult)await _mediator.Send(new CreateBookCommand(bookDto, User)); ;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody]BookUpdateDto book, int id)
        {
            return (IActionResult)await _mediator.Send(new UpdateBookCommand(id, book, User)); ;
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return (IActionResult)await _mediator.Send(new DeleteBookCommand(id, User)); ;
        }
    }

}
