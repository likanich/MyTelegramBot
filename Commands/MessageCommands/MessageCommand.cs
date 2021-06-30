using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands.MessageCommands
{
    public abstract class MessageCommand : ICommand
    {
        public abstract string Name { get; }

        public abstract Task Execute(string message, long chatId, TelegramBotClient client);

        public virtual bool Contains(Message message)
        {
            return message.Text.StartsWith(this.Name);
        }
    }
}
