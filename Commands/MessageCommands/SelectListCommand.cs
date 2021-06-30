using MyTelegramBot.BLL;
using NLog;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyTelegramBot.Commands.MessageCommands
{
    class SelectListCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ShoppingListService _shoppingListService;

        public SelectListCommand(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        private readonly string _callbackDataText = "$select_list ";
        public override string Name => "/select_list";

        public override async Task Execute(string message, long chatId, TelegramBotClient client)
        {
            _logger.Info($"Execute {Name} command. Chat id: {chatId}");

            try
            {
                var shoppingLists = _shoppingListService.GetAll(chatId);

                List<InlineKeyboardButton[]> buttons = new();
                foreach (var list in shoppingLists)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData(list.ListName, _callbackDataText + list.ListId) });
                }

                InlineKeyboardMarkup inlineKeyboard = new(buttons);
                await client.SendTextMessageAsync(chatId: chatId,
                                                  text: "Choose shopping list:",
                                                  replyMarkup: inlineKeyboard);
                _logger.Info($"Command {Name} completed successfully for chat id: {chatId}");
            }
            catch (CommandException ce)
            {
                await client.SendTextMessageAsync(chatId, $"Failed to select list. {ce.Message}.");
                _logger.Error(ce, $"Failed to select list. {ce.Message}. Chat id: {chatId}");
            }
        }
    }
}
