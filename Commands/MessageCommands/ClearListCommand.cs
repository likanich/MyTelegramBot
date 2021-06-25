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
    class ClearListCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public override string Name => @"/clear";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            using var context = new ShoppingListContext();

            try
            {
                var shoppingList = context.ShoppingLists.Include(s => s.Items).Where(s => s.UserId == chatId && s.IsSelected == true).FirstOrDefault();
                if (shoppingList == null)
                {
                    await client.SendTextMessageAsync(chatId, $"Failed to clear list. Shopping list not found. Try again.");
                    _logger.Warn($"Failed to clear list. List not found. Chat id: {chatId}");
                    return;
                }

                if (shoppingList.Items.Count == 0)
                {
                    await client.SendTextMessageAsync(chatId, $"Shopping list is empty");
                    _logger.Warn($"Failed to clear list. List is empty. Chat id: {chatId}");
                }
                else
                {
                    var items = shoppingList.Items;

                    context.Items.RemoveRange(items);
                    try
                    {
                        context.SaveChanges();
                        await client.SendTextMessageAsync(chatId, $"Shopping list {shoppingList.ListName} is clear");
                        _logger.Info($"Shopping list {shoppingList.ListName} is clear. Chat id: {chatId}");
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
