using MyTelegramBot.BLL;
using NLog;
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
        private readonly ShoppingListService _shoppingListService;

        public RenameListCommand(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        public string ReplyText => "Enter new list name:";

        public override string Name => @"/rename_list";

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute {Name} command. Chat id: {chatId}");
            try
            {
                var newListName = message.Contains(Name) ? message[Name.Length..].Trim() : message.Trim();

                if (string.IsNullOrEmpty(newListName))
                {
                    await client.SendTextMessageAsync(chatId, ReplyText, replyMarkup: new ForceReplyMarkup { Selective = true });
                    _logger.Info($"Send reply message for enter new list name. Chat id: {chatId}");
                    return;
                }

                var shoppingList = _shoppingListService.Get(chatId);
                var oldListName = shoppingList.ListName;
                shoppingList.ListName = newListName;
                _shoppingListService.Update(shoppingList);
                await client.SendTextMessageAsync(chatId, $"List {oldListName} is renamed to {shoppingList.ListName}");
                _logger.Info($"List {oldListName} is renamed to {shoppingList.ListName}. Chat id: {chatId}");
            }
            catch (CommandException ce)
            {
                await client.SendTextMessageAsync(chatId, $"Failed to rename list. {ce.Message}.");
                _logger.Error(ce, $"Failed rename shopping list. {ce.Message}. Chat id: {chatId}");
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
