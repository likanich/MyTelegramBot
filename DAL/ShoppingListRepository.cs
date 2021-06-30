using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Context;
using MyTelegramBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTelegramBot.DAL
{
    class ShoppingListRepository : IRepository<ShoppingList>, IDisposable
    {
        private readonly ApplicationContext _context;

        public ShoppingListRepository(ApplicationContext context)
        {
            _context = context;
        }

        public void Add(ShoppingList shoppingList)
        {
            _context.ShoppingLists.Add(shoppingList);
        }

        public void Delete(ShoppingList shoppingList)
        {
            _context.ShoppingLists.Remove(shoppingList);
        }

        public ShoppingList Get(int id)
        {
            var shoppingList = _context.ShoppingLists.Find(id);
            return shoppingList;
        }
        public ShoppingList GetByUserId(long userId)
        {
            var shoppingList = _context.ShoppingLists.Include(s => s.Items).Where(s => s.UserId == userId && s.IsSelected).FirstOrDefault();
            return shoppingList;
        }

        public IEnumerable<ShoppingList> GetAll(long userId)
        {
            var shoppingLists = _context.ShoppingLists.Include(s => s.Items).Where(s => s.UserId == userId).ToList();
            return shoppingLists;
        }

        public void Update(ShoppingList shoppingList)
        {
            _context.ShoppingLists.Update(shoppingList);
        }

        public int Save()
        {
            return _context.SaveChanges();
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
