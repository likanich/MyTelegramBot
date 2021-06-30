using MyTelegramBot.BLL;
using NLog;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands.MessageCommands
{
    class AddItemCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ShoppingListService _shoppingListService;

        public AddItemCommand(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        public override string Name => throw new NotImplementedException();

        public override bool Contains(Message message) => throw new NotImplementedException();

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute add item command. Chat id: {chatId}");

            var itemName = message.Trim();
            if (string.IsNullOrEmpty(itemName))
            {
                await client.SendTextMessageAsync(chatId, $"Failed add item to list. Incorrect item name.");
                _logger.Warn($"Failed add item to list. Incorrect item name. Chat id: {chatId}");
                return;
            }

            try
            {
                var listName = _shoppingListService.AddItem(chatId, itemName);

                await client.SendTextMessageAsync(chatId, $"{itemName} added to {listName}.");
                _logger.Info($"Item {itemName} added to list {listName}");
            }
            catch (CommandException ce)
            {
                await client.SendTextMessageAsync(chatId, $"Failed to add item to list. {ce.Message}.");
                _logger.Error(ce, $"Failed add item to list. {ce.Message}. Chat id: {chatId}");
            }
        }
    }
}
