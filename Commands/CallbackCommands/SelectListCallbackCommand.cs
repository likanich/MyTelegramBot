using MyTelegramBot.BLL;
using NLog;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MyTelegramBot.Commands.CallbackCommands
{
    internal class SelectListCallbackCommand : CallbackCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ShoppingListService _shoppingListService;
        public override string Name => @"$select_list";

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            try
            {
                int listId = ParseListId(message);
                var listName = _shoppingListService.Select(listId);
                await client.SendTextMessageAsync(chatId, $"List {listName} is selected");
                _logger.Info($"List {listName} is selected. Chat id: {chatId}");
                 
            }
            catch (CommandException ce)
            {
                await client.SendTextMessageAsync(chatId, $"Failed to select list. {ce.Message}.");
                _logger.Error(ce, $"Failed to select list. {ce.Message}. Chat id: {chatId}");
            }
        }

        private int ParseListId(string message)
        {
            if (Int32.TryParse(message[Name.Length..], out int listId))
            {
                return listId;
            }
            throw new CommandException("Incorrect index");
        }
    }
}
