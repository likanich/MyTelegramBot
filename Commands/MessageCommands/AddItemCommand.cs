using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Context;
using MyTelegramBot.Entities;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands.MessageCommands
{
    class AddItemCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public override string Name => throw new NotImplementedException();

        public override bool Contains(Message message) => throw new NotImplementedException();

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            _logger.Info($"Execute add item command. Chat id: {chatId}");

            var itemName = message.Text.Trim();
            if (string.IsNullOrEmpty(itemName))
            {
                await client.SendTextMessageAsync(chatId, $"Failed add item to list. Incorrect item name.");
                _logger.Warn($"Failed add item to list. Incorrect item name. Chat id: {chatId}");
                return;
            }

            using var context = new ShoppingListContext();

            try
            {
                var shoppingList = context.ShoppingLists.Where(p => p.UserId == chatId && p.IsSelected == true).FirstOrDefault();
                if (shoppingList == null)
                {
                    await client.SendTextMessageAsync(chatId, $"Failed to add item. Shopping list not found. Try again.");
                    _logger.Warn($"Failed to add item. List not found. Chat id: {chatId}");
                    return;
                }

                var thing = new Item { ShoppingList = shoppingList, ItemName = itemName };
                context.Items.Add(thing);
                try
                {
                    context.SaveChanges();
                    await client.SendTextMessageAsync(chatId, $"{itemName} added to {shoppingList.ListName}.");
                    _logger.Info($"Item {itemName} added to list {shoppingList.ListName}");
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
