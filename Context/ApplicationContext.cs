using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Entities;

namespace MyTelegramBot.Context
{
    class ApplicationContext : DbContext
    {
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<Item> Items { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(Settings.ConnectionString);

    }
}
