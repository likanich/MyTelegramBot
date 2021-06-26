using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    /// <summary>
    /// Some constants
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Telegram bot key
        /// </summary>
        public const string Key = "key";

        /// <summary>
        /// Connection string to Postgresql
        /// </summary>
        public const string ConnectionString = "Host=localhost;Port=5433;Database=db;Username=;Password=";
    }
}
