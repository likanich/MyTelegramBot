using MyTelegramBot.BLL;
using MyTelegramBot.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MyTelegramBot.Commands.MessageCommands
{
    class UnitedListCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ShoppingListService _shoppingListService;

        public UnitedListCommand(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        public override string Name => @"/united_list";

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            try
            {
                var shoppingLists = _shoppingListService.GetAll(chatId);

                await client.SendTextMessageAsync(chatId, $"United list:");
                if (shoppingLists.Any())
                {
                    await client.SendTextMessageAsync(chatId: chatId,
                                                      text: GetItemsString(shoppingLists),
                                                      parseMode: ParseMode.Html);
                }
                _logger.Info($"Successfully show united list. Chat id: {chatId}");
            }
            catch (CommandException ce)
            {
                await client.SendTextMessageAsync(chatId, $"Failed to show united list. {ce.Message}.");
                _logger.Error(ce, $"Failed to show united shopping lists. {ce.Message}. Chat id: {chatId}");
            }
        }

        private static string GetItemsString(IEnumerable<ShoppingList> shoppingLists)
        {
            StringBuilder builder = new();
            int i = 1;
            foreach (var shoppingList in shoppingLists)
            {
                foreach (var item in shoppingList.Items)
                {
                    if (item.IsBought)
                        builder.Append($"{i++}. <del><i>{item.ItemName}</i></del> <i>({shoppingList.ListName})</i>;{Environment.NewLine}");
                    else
                        builder.Append($"{i++}. {item.ItemName}  <i>({shoppingList.ListName})</i>;{Environment.NewLine}");
                }
            }
            return builder.ToString();
        }
    }
}
