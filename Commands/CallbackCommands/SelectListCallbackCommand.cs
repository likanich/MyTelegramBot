using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Context;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands.CallbackCommands
{
    internal class SelectListCallbackCommand : CallbackCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public override string Name => @"$select_list";

        public override async Task Execute(CallbackQuery callbackQuery, TelegramBotClient client)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            if (!Int32.TryParse(callbackQuery.Data[Name.Length..], out int listId))
            {
                await client.SendTextMessageAsync(chatId, $"Failed to select list. Try again.");
                _logger.Warn($"Failed to select list. Incorrect index. Chat id: {chatId}");
                return;
            }

            using var context = new ShoppingListContext();
            try
            {
                var newShoppingList = context.ShoppingLists.Where(p => p.UserId == chatId && p.ListId == listId).FirstOrDefault();
                if (newShoppingList == null)
                {
                    await client.SendTextMessageAsync(chatId, $"Failed to select list. Try again.");
                    _logger.Warn($"Failed to select list. List not found. Chat id: {chatId}");
                    return;
                }

                var oldShoppingList = context.ShoppingLists.Where(p => p.UserId == chatId && p.IsSelected).FirstOrDefault();
                if (oldShoppingList == null || oldShoppingList.Equals(newShoppingList))
                {
                    await client.SendTextMessageAsync(chatId, $"List {newShoppingList.ListName} is already selected");
                    _logger.Info($"List {newShoppingList.ListName} is already selected. Chat id: {chatId}");
                }
                else
                {
                    oldShoppingList.IsSelected = false;
                    newShoppingList.IsSelected = true;
                    context.ShoppingLists.UpdateRange(oldShoppingList, newShoppingList);
                    try
                    {
                        context.SaveChanges();

                        await client.SendTextMessageAsync(chatId, $"List {newShoppingList.ListName} is selected");
                        _logger.Info($"List {newShoppingList.ListName} is already selected. Chat id: {chatId}");
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
