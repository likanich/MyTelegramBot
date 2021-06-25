using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Context;
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
    class RenameListCommand : MessageCommand, IReplyCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public string ReplyText => "Enter new list name:";

        public override string Name => @"/rename_list";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            _logger.Info($"Execute {Name} command. Chat id: {chatId}");

            using var context = new ShoppingListContext();

            try
            {
                var newListName = message.Text.Contains(Name) ? message.Text[Name.Length..].Trim() : message.Text.Trim();

                if (string.IsNullOrEmpty(newListName))
                {
                    await client.SendTextMessageAsync(chatId, ReplyText, replyMarkup: new ForceReplyMarkup { Selective = true });
                    _logger.Info($"Send reply message for enter new list name. Chat id: {chatId}");
                    return;
                }

                var shoppingList = context.ShoppingLists.Where(s => s.UserId == chatId && s.IsSelected == true).FirstOrDefault();
                var oldListName = shoppingList.ListName;
                shoppingList.ListName = newListName;
                context.ShoppingLists.Update(shoppingList);
                try
                {
                    context.SaveChanges();
                    await client.SendTextMessageAsync(chatId, $"List {oldListName} is renamed to {shoppingList.ListName}");
                    _logger.Error($"List {oldListName} is renamed to {shoppingList.ListName}. Chat id: {chatId}");
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
