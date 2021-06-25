using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Context;
using MyTelegramBot.Entities;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyTelegramBot.Commands.MessageCommands
{
    class AddListCommand : MessageCommand, IReplyCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public override string Name => @"/add_list";

        public string ReplyText => $"Enter list name:";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            using var context = new ShoppingListContext();
            try
            {
                var listName = message.Text.Contains(Name) ? message.Text[Name.Length..].Trim() : message.Text.Trim();

                if (string.IsNullOrEmpty(listName))
                {
                    await client.SendTextMessageAsync(chatId, ReplyText, replyMarkup: new ForceReplyMarkup { Selective = true });
                    _logger.Info($"Send reply message for enter name for new list. Chat id: {chatId}");
                    return;
                }

                if (context.ShoppingLists.Where(p => p.ListName == listName && p.UserId == message.From.Id).Any())
                {
                    await client.SendTextMessageAsync(chatId, $"List {listName} exists!");
                    _logger.Warn($"Failed to create new list. List {listName} exists! Chat id: {chatId}");
                }
                else
                {
                    var shoppingLists = context.ShoppingLists.Where(p => p.UserId == message.From.Id);
                    foreach (var list in shoppingLists)
                    {
                        list.IsSelected = false;

                    }
                    context.ShoppingLists.UpdateRange(shoppingLists);
                    var shoppingList = new ShoppingList { ListName = listName, UserId = message.From.Id, IsSelected = true };
                    context.ShoppingLists.Add(shoppingList);
                    try
                    {
                        context.SaveChanges();
                        await client.SendTextMessageAsync(chatId, $"List {listName} added!");
                        _logger.Info($"List {listName} added! Chat id: {chatId}");
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

        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            if (message.ReplyToMessage != null && message.ReplyToMessage.Text.Contains(ReplyText))
            {
                return true;
            }

            return message.Text.Contains(this.Name);
        }
    }
}
