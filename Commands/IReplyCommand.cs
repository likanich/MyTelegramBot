namespace MyTelegramBot.Commands
{
    interface IReplyCommand
    {
        /// <summary>
        /// Name for reply command
        /// </summary>
        string ReplyText { get; }
    }
}
