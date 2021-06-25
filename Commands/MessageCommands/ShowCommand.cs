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
    class ShowCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public override string Name => "/show";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            await client.SendChatActionAsync(chatId, ChatAction.Typing);
            using var context = new ShoppingListContext();

            try
            {
                var shoppingList = context.ShoppingLists.Include(s => s.Items).Where(s => s.UserId == chatId && s.IsSelected).FirstOrDefault();
                if (shoppingList == null)
                {
                    _logger.Warn($"Not found shopping list for chat id: {chatId}. When executing command {Name}");
                    await client.SendTextMessageAsync(chatId, $"Not found shopping list. Try to add new Shopping list");
                    return;
                }
                StringBuilder builder = new();
                int i = 1;
                foreach (var item in shoppingList.Items)
                {
                    if (item.IsBought)
                        builder.Append($"{i++}. <del><i>{item.ItemName}</i></del>\n");
                    else
                        builder.Append($"{i++}. {item.ItemName};\n");
                }

                await client.SendTextMessageAsync(chatId, $"{shoppingList.ListName} ({shoppingList.Items.Count}):");
                if (shoppingList.Items.Count > 0)
                {
                    await client.SendTextMessageAsync(chatId: chatId,
                                                      text: builder.ToString(),
                                                      parseMode: ParseMode.Html);
                }
                _logger.Info($"Command {Name} completed successfully for chat id: {chatId}");
            }
            catch (ArgumentNullException ex)
            {
                _logger.Error(ex, $"Argument can't be null when getting shopping lists for chat id: {chatId}");
            }
        }
    }
}
