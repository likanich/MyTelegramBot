using System;
using System.Collections.Generic;

namespace MyTelegramBot.DAL
{
    interface IRepository<T> : IDisposable where T : class
    {
        IEnumerable<T> GetAll(long userId);
        T Get(int id);
        void Add(T item);
        void Delete(T item);
        void Update(T item);
        int Save();

    }
}
