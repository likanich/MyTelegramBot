using MyTelegramBot.Commands;
using System.Net.Http;
using Telegram.Bot;

namespace MyTelegramBot.Entities
{
    class MyTelegramBotClient : TelegramBotClient
    {
        public ICommand Command { private get; set; }

        public MyTelegramBotClient(string token, HttpClient httpClient = null, string baseUrl = null) : base(token, httpClient, baseUrl)
        {
        }

        public void RunCommand(string message, long chatId)
        {
            Command.Execute(message, chatId, this);
        }
    }
}
