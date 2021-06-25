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
    class DeleteListCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public override string Name => @"/delete_list";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            using var context = new ShoppingListContext();

            try
            {
                if (context.ShoppingLists.Where(s => s.UserId == chatId).Count() <= 1)
                {
                    await client.SendTextMessageAsync(chatId, $"You have only one shopping list. You can't delete it. For clean this list use command /clear");
                    _logger.Warn($"Can't delete one shopping list. Chat id: {chatId}");
                    return;
                }
                var shoppingList = context.ShoppingLists.Include(s => s.Items).Where(s => s.UserId == chatId && s.IsSelected).FirstOrDefault();
                var listName = shoppingList.ListName;
                context.ShoppingLists.Remove(shoppingList);
                try
                {
                    context.SaveChanges();
                    await client.SendTextMessageAsync(chatId, $"{listName} is delete");
                    _logger.Info($"List {listName} is delete. Chat id: {chatId}");
                    shoppingList = context.ShoppingLists.Include(s => s.Items).Where(s => s.UserId == chatId).FirstOrDefault();
                    shoppingList.IsSelected = true;
                    context.ShoppingLists.Update(shoppingList);
                    context.SaveChanges();
                    await client.SendTextMessageAsync(chatId, $"{shoppingList.ListName} is selected");
                    _logger.Info($"Command {Name} completed successfully. Chat id: {chatId}");
                }
                catch (DbUpdateException ex)
                {
                    _logger.Error(ex, $"Failed to save changes to database. Chat id: {chatId}");
                }
                
            }
            catch (ArgumentNullException ex)
            {
                _logger.Error(ex, $"Argument can't be null when getting shopping lists. Chat id: {chatId}");
            }
        }
    }
}
