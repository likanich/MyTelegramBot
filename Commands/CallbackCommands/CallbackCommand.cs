using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands.CallbackCommands
{
    public abstract class CallbackCommand : ICallbackCommand
    {
        public abstract string Name { get; }

        public abstract Task Execute(CallbackQuery callbackQuery, TelegramBotClient client);

        public virtual bool Contains(CallbackQuery callbackQuery)
        {
            return callbackQuery.Data.Contains(this.Name);
        }
    }
}
