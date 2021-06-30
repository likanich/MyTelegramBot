using MyTelegramBot.BLL;
using NLog;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MyTelegramBot.Commands.MessageCommands
{
    class StartCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ShoppingListService _shoppingListService;

        public StartCommand(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        public override string Name => "/start";
        private static readonly string _defaultListName = "Default";

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            CreateDefaultList(chatId);
            await client.SendTextMessageAsync(chatId, $"Hello! It is yet another shopping list bot. You can use next commands:");
            await client.SendTextMessageAsync(chatId, "/show - show shopping list\n" +
                                                        "meat - add meat to shopping list\n" +
                                                        "+4 - mark the 4th position as purchased\n" +
                                                        "-3 - remove the 3th position from the shopping list\n" +
                                                        "/clear - clear shopping list\n" +
                                                        "/united_list - show all shopping lists in one\n" +
                                                        "/bought_all - mark all list items as purchased\n" +
                                                        "/add_list - add new list\n" +
                                                        "/delete_list - delete current shopping list\n" +
                                                        "/select_list - select other shopping list\n" +
                                                        "/rename_list - rename current shopping list");
            _logger.Info($"Command {Name} completed successfully for chat id: {chatId}");

        }

        private void CreateDefaultList(long chatId)
        {
            try
            {
                _ = _shoppingListService.Get(chatId);
            }
            catch (CommandException ce)
            {
                _shoppingListService.Add(_defaultListName, chatId);
                _logger.Warn(ce, $"Not found shopping lists for chat id: {chatId}. Created shopping list with name {_defaultListName}");
            }
        }
    }
}
