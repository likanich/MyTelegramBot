using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands
{
    interface IMessageCommand
    {
        string Name { get; }

        Task Execute(Message message, TelegramBotClient client);
    }
}
