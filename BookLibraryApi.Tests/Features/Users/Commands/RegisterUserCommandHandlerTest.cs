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
            public static IEnumerable<object[]> RegisterUserCases =>
                new List<object[]>
                {
            new object[] { "abc", "abc", true },
            new object[] { "abc", "abc", false },
            new object[] { "ghost", null, false }
                };

            [Theory]
            [MemberData(nameof(RegisterUserCases))]
            public async Task RegisterUserTest(string username, string? password, bool shouldSucceed)
            {
                var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
                var mapper = TestFactory.CreateMapper();
                var hasher = new PasswordHasher<User>();

                if (!shouldSucceed && password != null)
                {
                    var user = TestFactory.CreateTestUser(username, password, out _);
                    await TestFactory.AddUsersAsync(context, user);
                }

                var dto = new RegisterUserDto { Username = username, Password = password };
                var command = new RegisterUserCommand(dto);
                var handler = new RegisterUserCommandHandler(context, mapper, hasher);

                var result = await handler.Handle(command, CancellationToken.None);

                if (shouldSucceed)
                    Assert.IsType<Created>(result);
                else if (password == null)
                    Assert.IsType<BadRequest>(result);
                else
                    Assert.IsType<Conflict<string>>(result);
            }
        }

    }
