using MyTelegramBot.BLL;
using MyTelegramBot.Commands;
using MyTelegramBot.Commands.CallbackCommands;
using MyTelegramBot.Commands.MessageCommands;
using MyTelegramBot.Context;
using MyTelegramBot.DAL;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MyTelegramBot.Entities
{
    /// <summary>
    /// The bot class
    /// Contains all methods to get messages from telegram
    /// </summary>
    public static class Bot
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static MyTelegramBotClient _botClient;
        private static List<MessageCommand> _commandsList;
        private static List<CallbackCommand> _callbackCommandsList;

        private static readonly ApplicationContext _applicationContext = new();
        private static readonly ShoppingListService _shoppingListService = new(new ShoppingListRepository(_applicationContext), new ItemRepository(_applicationContext));

        internal static ShoppingListService ShoppingListService => _shoppingListService;

        /// <summary>
        /// Returns an object <c>TelegramBotClient</c> or creates a new one if it hasn't been created yet
        /// </summary>
        /// <returns>Telegram bot client</returns>
        public static ITelegramBotClient GetBotClient()
        {
            if (_botClient != null)
            {
                return _botClient;
            }

            _commandsList = new List<MessageCommand>
            {
                new StartCommand(_shoppingListService),
                new AddListCommand(_shoppingListService),
                new SelectListCommand(_shoppingListService),
                new ShowCommand(_shoppingListService),
                new BuyCommand(_shoppingListService),
                new DeleteItemCommand(_shoppingListService),
                new ClearListCommand(_shoppingListService),
                new BoughtAllCommand(_shoppingListService),
                new UnitedListCommand(_shoppingListService),
                new DeleteListCommand(_shoppingListService),
                new RenameListCommand(_shoppingListService)
            };

            _callbackCommandsList = new List<CallbackCommand>
            {
                new SelectListCallbackCommand(_shoppingListService)
            };

            _botClient = new MyTelegramBotClient(Settings.Key);

            return _botClient;
        }

        public static DefaultUpdateHandler GetUpdateHandler()
        {
            return new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync);
        }

        /// <summary>
        /// Adds methods to receive different types of messages
        /// </summary>
        /// <param name="botClient">A client to use Telegram bot API</param>
        /// <param name="update">Incoming update</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns></returns>
        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(update.Message),
                UpdateType.EditedMessage => BotOnMessageReceived(update.Message),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery),
                _ => UnknownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static Task UnknownUpdateHandlerAsync(Update update)
        {
            throw new NotImplementedException();
        }

        private static async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            _logger.Info($"Receive callback querry: {callbackQuery.Message.Type}");
            var message = callbackQuery.Message;
            if (message == null || message.Type != MessageType.Text)
            {
                _logger.Warn("Received callback query message is null or type of message unknow");
                return;
            }

            if (SetBotCallbackCommand(callbackQuery))
                _botClient.RunCommand(callbackQuery.Data, message.Chat.Id);
        }

        private static bool SetBotCallbackCommand(CallbackQuery callbackQuery)
        {
            foreach (var command in _callbackCommandsList)
            {
                if (command.Contains(callbackQuery))
                {
                    _botClient.Command = command;
                    return true;
                }
            }
            return false;
        }

        private static async Task BotOnMessageReceived(Message message)
        {
            _logger.Info($"Receive message type: {message.Type}");
            if (message == null || message.Type != MessageType.Text)
            {
                _logger.Warn("Received message is null or type of message unknow");
                return;
            }

            SetBotMessageCommand(message);
            _botClient.RunCommand(message.Text, message.Chat.Id);
        }

        private static void SetBotMessageCommand(Message message)
        {
            if (message.ReplyMarkup != null)
            {
                foreach (var command in _commandsList)
                {
                    if (command is IReplyCommand && command.Contains(message))
                    {
                        _botClient.Command = command;
                        return;
                    }
                }
            }
            foreach (var command in _commandsList)
            {
                if (command.Contains(message))
                {
                    _botClient.Command = command;
                    return;
                }
            }
            _botClient.Command = new AddItemCommand(_shoppingListService);
        }

        private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                await botClient.SendTextMessageAsync(123, apiRequestException.ToString(), cancellationToken: cancellationToken);
            }
        }
    }
}
