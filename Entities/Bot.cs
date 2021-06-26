using MyTelegramBot.Commands.MessageCommands;
using MyTelegramBot.Commands.CallbackCommands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MyTelegramBot.Commands;
using NLog;

namespace MyTelegramBot.Entities
{
    /// <summary>
    /// The bot class
    /// Contains all methods to get messages from telegram
    /// </summary>
    public static class Bot
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static TelegramBotClient botClient;
        private static List<MessageCommand> commandsList;
        private static List<CallbackCommand> callbackCommandsList;

        public static IReadOnlyList<MessageCommand> Commands => commandsList.AsReadOnly();
        public static IReadOnlyList<CallbackCommand> CallbackCommands => callbackCommandsList.AsReadOnly();

        /// <summary>
        /// Returns an object <c>TelegramBotClient</c> or creates a new one if it hasn't been created yet
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns>Telegram bot client</returns>
        public static async Task<TelegramBotClient> GetBotClientAsync(CancellationToken cancellationToken)
        {
            if (botClient != null)
            {
                return botClient;
            }

            commandsList = new List<MessageCommand>
            {
                new StartCommand(),
                new AddListCommand(),
                new SelectListCommand(),
                new ShowCommand(),
                new BuyCommand(),
                new DeleteItemCommand(),
                new ClearListCommand(),
                new BoughtAllCommand(),
                new UnitedListCommand(),
                new DeleteListCommand(),
                new RenameListCommand()
            };

            callbackCommandsList = new List<CallbackCommand>
            {
                new SelectListCallbackCommand()
            };

            botClient = new TelegramBotClient(Settings.Key);

            botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cancellationToken);
            return botClient;
        }

        /// <summary>
        /// Adds methods to receive different types of messages
        /// </summary>
        /// <param name="botClient">A client to use Telegram bot API</param>
        /// <param name="update">Incoming update</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        /// <returns></returns>
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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

            foreach (var command in Bot.CallbackCommands)
            {
                if (command.Contains(callbackQuery))
                {
                    await command.Execute(callbackQuery, botClient);
                    return;
                }
            }
        }

        private static async Task BotOnMessageReceived(Message message)
        {
            _logger.Info($"Receive message type: {message.Type}");
            if (message == null || message.Type != MessageType.Text)
            {
                _logger.Warn("Received message is null or type of message unknow");
                return;
            }

            var commands = Bot.Commands;
            if (message.ReplyMarkup != null)
            {
                foreach (var command in commands)
                {
                    if (command is IReplyCommand && command.Contains(message))
                    {
                        await command.Execute(message, botClient);
                        return;
                    }
                }
            }
            foreach (var command in commands)
            {
                if (command.Contains(message))
                {
                    await command.Execute(message, botClient);
                    return;
                }
            }
            _ = new AddItemCommand().Execute(message, botClient);
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
