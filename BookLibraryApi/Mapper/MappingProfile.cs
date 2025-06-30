using AutoMapper;
using BookLibraryApi.Models;

namespace BookLibraryApi.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<BookCreateDto, Book>();
            CreateMap<Book, BookReadDto>();
            CreateMap<RegisterUserDto, User>();
        }
    }
}
