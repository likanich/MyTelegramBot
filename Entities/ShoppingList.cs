using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyTelegramBot.Entities
{
    class ShoppingList
    {
        [Key]
        public int ListId { get; set; }
        public string ListName { get; set; }
        public long UserId { get; set; }
        public bool IsSelected { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();
    }
}
