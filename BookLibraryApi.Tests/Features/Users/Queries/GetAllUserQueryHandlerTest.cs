using BookLibraryApi.Tests.Common;
using BookLibraryApi.Features.Users.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryApi.Mapper;
using Microsoft.AspNetCore.Http.HttpResults;
using BookLibraryApi.Models.Dtos;

namespace BookLibraryApi.Tests.Features.Users.Queries
{
    public class GetAllUserQueryHandlerTest
    {
        [Theory]
        [InlineData("abc",1,"Admin", true)]
        [InlineData("abc",2,"User", false)]
        public async Task GetAllUsers(string Username,int? id, string Role, bool ShouldSucced )
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var mapper = new AutoMapper.Mapper(new MapperConfiguration(cfg =>
                cfg.AddProfile<MappingProfile>()
            ));
            var claim = TestFactory.CreateClaimsPrincipal(Username, id, Role);


            var querry = new GetAllUsersQuery(claim);
            var handler = new GetAllUsersQueryHandler(context, mapper);
            
            var  result = await handler.Handle(querry, CancellationToken.None);

            if (ShouldSucced)
                Assert.IsType<Ok<List<UserReadDto>>>(result);
            else
                 Assert.IsType<ForbidHttpResult>(result);
        }
    }
}
