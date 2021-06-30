using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Context;
using MyTelegramBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTelegramBot.DAL
{
    class ItemRepository : IRepository<Item>, IDisposable
    {
        private readonly ApplicationContext _context;

        public ItemRepository(ApplicationContext context)
        {
            _context = context;
        }

        public void Add(Item item)
        {
            _context.Items.Add(item);
        }

        public void Delete(Item item)
        {
            _context.Items.Remove(item);
        }

        public void DeleteRange(IEnumerable<Item> items)
        {
            _context.RemoveRange(items);
        }

        public Item Get(int id)
        {
            return _context.Items.Find(id);
        }

        public IEnumerable<Item> GetAll(long userId)
        {
            return _context.ShoppingLists.Include(s => s.Items).Where(s => s.UserId == userId && s.IsSelected).FirstOrDefault().Items.ToList();
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public void Update(Item item)
        {
            _context.Update(item);
        }

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                _disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
