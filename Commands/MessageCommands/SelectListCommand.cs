using MyTelegramBot.Context;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyTelegramBot.Commands.MessageCommands
{
    class SelectListCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly string _callbackDataText = "$select_list ";
        public override string Name => "/select_list";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            _logger.Info($"Execute {Name} command. Chat id: {chatId}");

            await client.SendChatActionAsync(chatId, ChatAction.Typing);
            using var context = new ShoppingListContext();

            try
            {
                var shoppingLists = context.ShoppingLists.Where(p => p.UserId == chatId);
                if (shoppingLists == null)
                {
                    _logger.Warn($"Not found shopping lists. Chat id: {chatId}.");
                    await client.SendTextMessageAsync(chatId, $"Not found shopping lists. Try to add new Shopping list");
                    return;
                }
                List<InlineKeyboardButton[]> buttons = new();

                foreach (var list in shoppingLists)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData(list.ListName, _callbackDataText + list.ListId) });
                }

                InlineKeyboardMarkup inlineKeyboard = new(buttons);
                await client.SendTextMessageAsync(chatId: message.Chat.Id,
                                                          text: "Choose shopping list:",
                                                          replyMarkup: inlineKeyboard);
                _logger.Info($"Command {Name} completed successfully for chat id: {chatId}");
            }
            catch (ArgumentNullException ex)
            {
                _logger.Error(ex, $"Argument can't be null when getting shopping lists for chat id: {chatId}");
            }
        }
    }
}
