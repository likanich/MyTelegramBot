using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Context;
using NLog;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MyTelegramBot.Commands.MessageCommands
{
    class UnitedListCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public override string Name => @"/united_list";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            await client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            using var context = new ShoppingListContext();

            try
            {
                var shoppingLists = context.ShoppingLists.Include(s => s.Items).Where(s => s.UserId == chatId);
                if (shoppingLists == null)
                {
                    _logger.Warn($"Not found shopping lists. Chat id: {chatId}.");
                    await client.SendTextMessageAsync(chatId, $"Not found shopping lists. Try to add new Shopping list");
                    return;
                }
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

                await client.SendTextMessageAsync(chatId, $"United list:");
                if (i > 1)
                {
                    await client.SendTextMessageAsync(chatId: chatId,
                                                      text: builder.ToString(),
                                                      parseMode: ParseMode.Html);
                }
                _logger.Info($"Successfully show united list. Chat id: {chatId}");
            }
            catch (ArgumentNullException ex)
            {
                _logger.Error(ex, $"Argument can't be null when getting shopping lists. Chat id: {chatId}");
            }
        }
    }
}
