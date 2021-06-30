using MyTelegramBot.Entities;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace MyTelegramBot
{
    /// <summary>
    /// Main class for run Telegram bot
    /// </summary>
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static ITelegramBotClient _telegramBot;

        static async Task Main(string[] args)
        {
            _telegramBot = Bot.GetBotClient();

            var me = await _telegramBot.GetMeAsync();
            Console.Title = me.Username;

            var cts = new CancellationTokenSource();

            _telegramBot.StartReceiving(Bot.GetUpdateHandler(), cts.Token);
            _logger.Info($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
            _logger.Warn("Telegram bot for shopping list stopped");
        }
    }
}
