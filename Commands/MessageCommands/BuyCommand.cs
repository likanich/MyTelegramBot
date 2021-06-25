using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Context;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands.MessageCommands
{
    class BuyCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public override string Name => @"+";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            if (!Int32.TryParse(message.Text[Name.Length..], out int itemId))
            {
                await client.SendTextMessageAsync(chatId, $"Incorrect index");
                _logger.Warn($"Failed to delete item. Incorrect index. Chat id: {chatId}");
                return;
            }

            using var context = new ShoppingListContext();
            try
            {
                var shoppingList = context.ShoppingLists.Include(s => s.Items).Where(s => s.UserId == chatId && s.IsSelected == true).FirstOrDefault();
                if (shoppingList == null)
                {
                    await client.SendTextMessageAsync(chatId, $"Failed to buy item. Shopping list not found. Try again.");
                    _logger.Warn($"Failed to buy item. List not found. Chat id: {chatId}");
                    return;
                }

                var itemsCount = shoppingList.Items.Count;
                if (itemsCount == 0)
                {
                    await client.SendTextMessageAsync(chatId, $"Shopping list is empty");
                    _logger.Warn($"Failed to buy item. Shopping list is empty. Chat id: {chatId}");
                }
                else if (itemId <= 0 || itemId > itemsCount)
                {
                    await client.SendTextMessageAsync(chatId, $"Incorrect index. Please, enter number between 1 and {itemsCount}");
                    _logger.Warn($"Failed to buy item. Index {itemId} out of range. Chat id: {chatId}");
                }
                else
                {
                    var item = shoppingList.Items[itemId - 1];
                    item.IsBought = true;

                    context.Items.Update(item);
                    try
                    {
                        context.SaveChanges();
                        await client.SendTextMessageAsync(chatId, $"{item.ItemName} is bought");
                        _logger.Info($"Item {item.ItemName} is bought. Chat id: {chatId}");
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.Error(ex, $"Failed to save changes to database. Chat id: {chatId}");
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                _logger.Error(ex, $"Argument can't be null when getting shopping lists. Chat id: {chatId}");
            }
        }
    }
}
