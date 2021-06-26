using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands
{
    /// <summary>
    /// Interface for callback command from telegram client
    /// </summary>
    interface ICallbackCommand
    {
        /// <summary>
        /// Command name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Execute command from callback querry
        /// </summary>
        /// <param name="message">Telegram message</param>
        /// <param name="client">Telegram bot client</param>
        /// <returns></returns>
        Task Execute(CallbackQuery callbackQuery, TelegramBotClient client);
    }
}
