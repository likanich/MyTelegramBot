using MyTelegramBot.BLL;
using NLog;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MyTelegramBot.Commands.MessageCommands
{
    class BuyCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ShoppingListService _shoppingListService;

        public BuyCommand(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        public override string Name => @"+";

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            try
            {
                int itemNumber = ParseItemNumber(message);
                var itemName = _shoppingListService.ByuItem(chatId, itemNumber);
                await client.SendTextMessageAsync(chatId, $"{itemName} is bought");
                _logger.Info($"Item {itemName} is bought. Chat id: {chatId}");
            }
            catch (CommandException ce)
            {
                await client.SendTextMessageAsync(chatId, $"Failed to buy item. {ce.Message}.");
                _logger.Error(ce, $"Failed to buy item. {ce.Message}. Chat id: {chatId}");
            }
        }

        private int ParseItemNumber(string message)
        {
            if (Int32.TryParse(message[Name.Length..], out int itemNumber))
            {
                return itemNumber;
            }
            throw new CommandException("Incorrect index");
        }
    }
}
