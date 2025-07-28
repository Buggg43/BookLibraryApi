using AutoMapper;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;

namespace BookLibraryApi.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<BookCreateDto, Book>()
                .ForMember(b => b.Id, opt => opt.Ignore())
                .ForMember(b => b.isRead, opt => opt.Ignore())
                .ForMember(b => b.isFavorite, opt => opt.Ignore())
                .ForMember(b => b.UserId, opt => opt.Ignore())
                .ForMember(b => b.User, opt => opt.Ignore());

            CreateMap<Book, BookReadDto>();

            CreateMap<RegisterUserDto, User>()
                .ForMember(u => u.Id, opt => opt.Ignore())
                .ForMember(u => u.PasswordHash, opt => opt.Ignore())
                .ForMember(u => u.Role, opt => opt.Ignore())
                .ForMember(u => u.RefreshTokens, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForMember(u => u.Id, opt => opt.Ignore())
                .ForMember(u => u.PasswordHash, opt => opt.Ignore())
                .ForMember(u => u.Role, opt => opt.Ignore())
                .ForMember(u => u.RefreshTokens, opt => opt.Ignore());

            CreateMap<User, UserReadDto>();

            CreateMap<BookUpdateDto, Book>()
                .ForMember(b => b.Id, opt => opt.Ignore())
                .ForMember(b => b.UserId, opt => opt.Ignore())
                .ForMember(b => b.User, opt => opt.Ignore());
        }
    }
}
