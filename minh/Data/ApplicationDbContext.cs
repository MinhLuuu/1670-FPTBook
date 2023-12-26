using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using minh.Models;

namespace minh.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<minh.Models.Genre>? Genre { get; set; }
        public DbSet<minh.Models.Book>? Book { get; set; }
        public DbSet<minh.Models.Cart>? Cart { get; set; }
        public DbSet<minh.Models.Order>? Order { get; set; }
    }
}