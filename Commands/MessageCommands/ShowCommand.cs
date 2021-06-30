using MyTelegramBot.BLL;
using MyTelegramBot.Entities;
using NLog;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MyTelegramBot.Commands.MessageCommands
{
    class ShowCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ShoppingListService _shoppingListService;

        public ShowCommand(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        public override string Name => "/show";

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute {Name} command. Chat id: {chatId}");

            try
            {
                var shoppingList = _shoppingListService.Get(chatId);

                await client.SendTextMessageAsync(chatId, $"{shoppingList.ListName} ({shoppingList.Items.Count}):");
                if (shoppingList.Items.Count > 0)
                {
                    await client.SendTextMessageAsync(chatId: chatId,
                                                      text: GetItemsString(shoppingList.Items),
                                                      parseMode: ParseMode.Html);
                }
                _logger.Info($"Successfully show list {shoppingList.ListName}. Chat id: {chatId}");
            }
            catch (CommandException ce)
            {
                await client.SendTextMessageAsync(chatId, $"Failed to show list. {ce.Message}.");
                _logger.Error(ce, $"Failed to show shopping list. {ce.Message}. Chat id: {chatId}");
            }
        }

        private static string GetItemsString(IEnumerable<Item> items)
        {
            StringBuilder builder = new();
            int i = 1;
            foreach (var item in items)
            {
                if (item.IsBought)
                    builder.Append($"{i++}. <del><i>{item.ItemName}</i></del>\n");
                else
                    builder.Append($"{i++}. {item.ItemName};\n");
            }
            return builder.ToString();
        }
    }
}
