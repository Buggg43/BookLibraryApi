using AutoMapper;
using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Mapper;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Tests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryApi.Tests.Features.Users.Commands
{
    public class RegisterUserCommandHandlerTest
    {
        [Theory]
        [InlineData("abc","abc", true)]
        [InlineData("abc", "abc", false)]
        [InlineData("ghost",null, false)]
        public async Task RegisterUserTest(string Username,string? Password, bool ShouldSucced)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var mapper = new AutoMapper.Mapper(new MapperConfiguration(cfg =>
                cfg.AddProfile<MappingProfile>()
            ));
            var hasher = new PasswordHasher<User>();

            var newUser = new RegisterUserDto { Username = Username, Password = Password};
            if(!ShouldSucced)
            {
                await context.Users.AddAsync(mapper.Map<User>(newUser));
                await context.SaveChangesAsync();
            }

            var command = new RegisterUserCommand(newUser);
            var handler = new RegisterUserCommandHandler(context, mapper, hasher);

            var result = await handler.Handle(command, CancellationToken.None);

            if (ShouldSucced)
            {
                Assert.IsType<Created>(result);
            }
            else if (Username == "ghost")
                Assert.IsType<BadRequest>(result);
            else
                Assert.IsType<Conflict<string>>(result);


        }
    }
}
