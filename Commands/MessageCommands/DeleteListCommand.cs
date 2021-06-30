using MyTelegramBot.BLL;
using NLog;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MyTelegramBot.Commands.MessageCommands
{
    class DeleteListCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ShoppingListService _shoppingListService;

        public DeleteListCommand(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        public override string Name => @"/delete_list";

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            try
            {
                var deletedListName = _shoppingListService.Delete(chatId);
                await client.SendTextMessageAsync(chatId, $"{deletedListName} is delete");
                _logger.Info($"List {deletedListName} is delete. Chat id: {chatId}");
                var selectedListName = _shoppingListService.Select(chatId);
                await client.SendTextMessageAsync(chatId, $"{selectedListName} is selected");
                _logger.Info($"{selectedListName} is selected. Chat id: {chatId}");
            }
            catch (CommandException ce)
            {
                await client.SendTextMessageAsync(chatId, $"Failed to delete list. {ce.Message}.");
                _logger.Error(ce, $"Failed to delete list. {ce.Message}. Chat id: {chatId}");
            }
        }
    }
}
