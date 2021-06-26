using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyTelegramBot.Entities
{
    /// <summary>
    /// Shopping list item
    /// </summary>
    class Item
    {
        [Key]
        public int ItemId { get; set; }
        public int ShoppingListId { get; set; }
        public ShoppingList ShoppingList { get; set; }
        public string ItemName { get; set; }
        [DefaultValue(false)]
        public bool IsBought { get; set; }
    }
}
