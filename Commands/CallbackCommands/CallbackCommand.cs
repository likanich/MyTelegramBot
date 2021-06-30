using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands.CallbackCommands
{
    public abstract class CallbackCommand : ICommand
    {
        public abstract string Name { get; }

        public abstract Task Execute(string message, long chatId, TelegramBotClient client);

        public virtual bool Contains(CallbackQuery callbackQuery)
        {
            return callbackQuery.Data.StartsWith(this.Name);
        }
    }
}
