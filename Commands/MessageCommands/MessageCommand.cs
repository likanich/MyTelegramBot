using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Commands.MessageCommands
{
	public abstract class MessageCommand : IMessageCommand
	{
        public abstract string Name { get; }

        public abstract Task Execute(Message message, TelegramBotClient client);

		public virtual bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
	}
}
