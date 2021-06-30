using MyTelegramBot.BLL;
using NLog;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MyTelegramBot.Commands.MessageCommands
{
    class BoughtAllCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ShoppingListService _shoppingListService;

        public BoughtAllCommand(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        public override string Name => @"/bought_all";

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute {Name} command. Chat id: {chatId}");

            try
            {
                var listName = _shoppingListService.BoughtAllItems(chatId);
                await client.SendTextMessageAsync(chatId, $"Everything is bought in list {listName}");
                _logger.Info($"Bought all item in list {listName}. Chat id: {chatId}");

            }
            catch (CommandException ce)
            {
                await client.SendTextMessageAsync(chatId, $"Failed to bought all items. {ce.Message}.");
                _logger.Error(ce, $"Failed to bought all items. {ce.Message} Chat id: {chatId}");
            }
        }
    }
}
