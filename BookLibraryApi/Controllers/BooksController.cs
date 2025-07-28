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

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all-books")]
        public async Task<IResult> GetAllBookFromAllUsers([FromQuery] GetUserBooksDto filters)
        {
            return await _mediator.Send(new GetAllBooksQuery(filters));
        }

        [HttpGet]
        public async Task<IResult> GetAll([FromQuery] GetUserBooksDto filters)
        {
            return await _mediator.Send(new GetUserBooksQuery(filters, User));
        }

        [HttpGet("{id}")]
        public async Task<IResult> GetById(int id)
        {
            return await _mediator.Send(new GetBookByIdQuery(id, User));
        }

        [HttpPost]
        public async Task<IResult> AddBook([FromBody] BookCreateDto bookDto)
        {
            return await _mediator.Send(new CreateBookCommand(bookDto, User));
        }

        [HttpPut("{id}")]
        public async Task<IResult> Update([FromBody] BookUpdateDto book, int id)
        {
            return await _mediator.Send(new UpdateBookCommand(id, book, User));
        }

        [HttpDelete("{id}")]
        public async Task<IResult> Delete(int id)
        {
            return await _mediator.Send(new DeleteBookCommand(id, User));
        }
    }
}
