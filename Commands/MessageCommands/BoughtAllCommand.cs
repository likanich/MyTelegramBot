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
    class BoughtAllCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public override string Name => @"/bought_all";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            _logger.Info($"Execute {Name} command. Chat id: {chatId}");

            using var context = new ShoppingListContext();

            try
            {
                var shoppingList = context.ShoppingLists.Include(s => s.Items).Where(s => s.UserId == chatId && s.IsSelected == true).FirstOrDefault();
                if (shoppingList == null)
                {
                    await client.SendTextMessageAsync(chatId, $"Failed to bought all. Shopping list not found. Try again.");
                    _logger.Warn($"Failed to bought all items. List not found. Chat id: {chatId}");
                    return;
                }

                if (shoppingList.Items.Count == 0)
                {
                    await client.SendTextMessageAsync(chatId, $"Shopping list is empty");
                    _logger.Warn($"Failed to bought all items. Shopping list is empty. Chat id: {chatId}");
                }
                else
                {
                    var items = shoppingList.Items;
                    foreach (var item in items)
                        item.IsBought = true;

                    context.Items.UpdateRange(items);
                    try
                    {
                        context.SaveChanges();
                        await client.SendTextMessageAsync(chatId, $"Everything is bought");
                        _logger.Info($"Bought all item in list {shoppingList.ListName}. Chat id: {chatId}");
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
