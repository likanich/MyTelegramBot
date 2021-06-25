using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands
{
    interface ICallbackCommand
    {
        string Name { get; }

        Task Execute(CallbackQuery callbackQuery, TelegramBotClient client);
    }
}
