using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands
{
    /// <summary>
    /// Interfase for message command from Telegram bot API
    /// </summary>
    interface IMessageCommand
    {
        /// <summary>
        /// Command name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Execute command from user text message
        /// </summary>
        /// <param name="message">Telegram message</param>
        /// <param name="client">Telegram bot client</param>
        /// <returns></returns>
        Task Execute(Message message, TelegramBotClient client);
    }
}
