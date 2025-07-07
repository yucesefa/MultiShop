using Microsoft.EntityFrameworkCore;
using MultiShop.Message.DAL.Entities;

namespace MultiShop.Message.DAL.Context
{
    public class MessageContext : DbContext
    {
        public MessageContext(DbContextOptions<MessageContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
        public DbSet<UserMessage> UserMessages { get; set; }
    }
}

