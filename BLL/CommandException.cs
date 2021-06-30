using System;

namespace MyTelegramBot.BLL
{
    class CommandException : Exception
    {
        public CommandException(string message) : base(message)
        {
        }
    }
}
