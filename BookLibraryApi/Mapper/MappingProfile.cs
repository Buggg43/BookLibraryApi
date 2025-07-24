using AutoMapper;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;

namespace BookLibraryApi.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<BookCreateDto, Book>();
            CreateMap<Book, BookReadDto>();
            CreateMap<RegisterUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UserReadDto>();
            CreateMap<BookUpdateDto, Book>();
        }
    }
}
