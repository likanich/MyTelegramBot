using MyTelegramBot.Entities;
using NLog;
using System;
using System.Threading;

namespace MyTelegramBot
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            Bot.GetBotClientAsync(cts.Token).Wait();
            _logger.Info("Telegram bot for shopping list started");

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            cts.Cancel();
            _logger.Warn("Telegram bot for shopping list stopped");
        }
    }
}
