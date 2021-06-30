using MyTelegramBot.BLL;
using NLog;
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
        private readonly ShoppingListService _shoppingListService;

        public AddListCommand(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        public override string Name => @"/add_list";

        public string ReplyText => $"Enter list name:";

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            var listName = GetListName(message);

            if (string.IsNullOrEmpty(listName))
            {
                await client.SendTextMessageAsync(chatId, ReplyText, replyMarkup: new ForceReplyMarkup { Selective = true });
                _logger.Info($"Send reply message for enter name for new list. Chat id: {chatId}");
                return;
            }

            try
            {
                _shoppingListService.Add(listName, chatId);
                await client.SendTextMessageAsync(chatId, $"List {listName} added!");
                _logger.Info($"List {listName} added! Chat id: {chatId}");
            }
            catch (CommandException ce)
            {
                await client.SendTextMessageAsync(chatId, $"List {listName} not added! {ce.Message}");
                _logger.Error(ce, $"Failed to save changes to database. Chat id: {chatId}");
            }

        }

        private string GetListName(string message)
        {
            return message.Contains(Name) ? message[Name.Length..].Trim() : message.Trim();
        }

        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            if (message.ReplyToMessage != null && message.ReplyToMessage.Text.StartsWith(ReplyText))
            {
                return true;
            }

            return message.Text.StartsWith(this.Name);
        }
    }
}
