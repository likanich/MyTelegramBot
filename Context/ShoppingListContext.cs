using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Entities;

namespace MyTelegramBot.Context
{
    class ShoppingListContext : DbContext
    {
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<Item> Items { get; set; }

        public ShoppingListContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(Settings.ConnectionString);

    }
}
