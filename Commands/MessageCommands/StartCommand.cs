using MyTelegramBot.Context;
using MyTelegramBot.Entities;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands.MessageCommands
{
    class StartCommand : MessageCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public override string Name => "/start";
        private static readonly string _defaultListName = "Default";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            _logger.Info($"Execute {Name} command for chat id: {chatId}");

            using var context = new ShoppingListContext();
            try
            {
                var shoppingLists = context.ShoppingLists.Where(p => p.UserId == chatId);
                if (shoppingLists.Count() == 0)
                {
                    var shoppingList = new ShoppingList { ListName = _defaultListName, UserId = message.From.Id, IsSelected = true };

                    context.ShoppingLists.Add(shoppingList);
                    context.SaveChanges();
                    _logger.Info($"Not found shopping lists for chat id: {chatId}. Created shopping list with name {_defaultListName}");
                }


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
            catch (ArgumentNullException ex)
            {
                _logger.Error(ex, $"Argument can't be null when getting shopping lists for chat id: {chatId}");
            }
        }

    }
}
