﻿using Microsoft.EntityFrameworkCore;
using BookLibraryApi.Models;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace BookLibraryApi.Data
{
    public class LibraryDbContext : DbContext
    {

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
