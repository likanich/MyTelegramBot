using System.Threading.Tasks;
using Telegram.Bot;

namespace MyTelegramBot.Commands
{
    interface ICommand
    {
        /// <summary>
        /// Command name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Execute command from user text message
        /// </summary>
        /// <param name="message">Telegram message</param>
        /// <param name="chatId">User Id</param>
        /// <param name="client">Telegram bot client</param>
        /// <returns></returns>
        Task Execute(string message, long chatId, TelegramBotClient client);

    }
}
