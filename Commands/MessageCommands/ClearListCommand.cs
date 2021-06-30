using MyTelegramBot.BLL;
using NLog;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MyTelegramBot.Commands.MessageCommands
{
    class ClearListCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ShoppingListService _shoppingListService;

        public ClearListCommand(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        public override string Name => @"/clear";

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            try
            {
                var shoppingListName = _shoppingListService.Clear(chatId);
                await client.SendTextMessageAsync(chatId, $"Shopping list {shoppingListName} is clear");
                _logger.Info($"Shopping list {shoppingListName} is clear. Chat id: {chatId}");
            }
            catch (CommandException ce)
            {
                await client.SendTextMessageAsync(chatId, $"Failed to clean list. {ce.Message}.");
                _logger.Error(ce, $"Failed to clean shopping list. {ce.Message}. Chat id: {chatId}");
            }
        }
    }
}
