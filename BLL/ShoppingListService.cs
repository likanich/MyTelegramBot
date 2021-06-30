using MyTelegramBot.DAL;
using MyTelegramBot.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MyTelegramBot.BLL
{
    class ShoppingListService
    {
        private readonly ShoppingListRepository _shoppingListRepository;
        private readonly ItemRepository _itemRepository;

        public ShoppingListService(ShoppingListRepository shoppingListRepository, ItemRepository itemRepository)
        {
            _shoppingListRepository = shoppingListRepository;
            _itemRepository = itemRepository;
        }

        public void Add(string listName, long chatId)
        {
            var shoppingLists = _shoppingListRepository.GetAll(chatId);
            foreach (var list in shoppingLists)
            {
                if (list.ListName.Equals(listName))
                    throw new CommandException($"List {listName} exists!");
            }

            foreach (var list in shoppingLists)
            {
                list.IsSelected = false;
                _shoppingListRepository.Update(list);
            }

            var shoppingList = new ShoppingList
            {
                ListName = listName,
                UserId = chatId,
                IsSelected = true
            };
            _shoppingListRepository.Add(shoppingList);
            if (_shoppingListRepository.Save() == 0)
                throw new CommandException("Not saved to database");

        }

        public IEnumerable<ShoppingList> GetAll(long chatId)
        {
            var shoppingLists = _shoppingListRepository.GetAll(chatId);
            if (!shoppingLists.Any())
            {
                throw new CommandException("Not found shopping lists");
            }
            return shoppingLists;
        }

        public string Delete(long chatId)
        {
            if (_shoppingListRepository.GetAll(chatId).Count() <= 1)
                throw new CommandException("This is only one shopping list. Can't delete it. For clean this list use command /clear");
            var shoppingList = _shoppingListRepository.GetByUserId(chatId);
            var listName = shoppingList.ListName;
            _shoppingListRepository.Delete(shoppingList);
            if (_shoppingListRepository.Save() > 0)
                return listName;
            else
                throw new CommandException("Not deleted from database");
        }

        public string Select(long chatId)
        {
            var shoppingList = _shoppingListRepository.GetAll(chatId).FirstOrDefault();
            shoppingList.IsSelected = true;
            var listName = shoppingList.ListName;
            _shoppingListRepository.Update(shoppingList);
            if (_shoppingListRepository.Save() > 0)
                return listName;
            else
                throw new CommandException("Not saved to database");
        }

        public string Select(long chatId, int listId)
        {
            var oldShoppingList = Get(chatId);
            var newShoppingList = _shoppingListRepository.Get(listId);
            if (newShoppingList == null)
            {
                throw new CommandException("Not found shopping list");
            }
            if (oldShoppingList.Equals(newShoppingList))
            {
                throw new CommandException($"List {newShoppingList.ListName} is already selected");
            }
            oldShoppingList.IsSelected = false;
            newShoppingList.IsSelected = true;
            _shoppingListRepository.Update(oldShoppingList);
            _shoppingListRepository.Update(newShoppingList);
            if (_shoppingListRepository.Save() > 0)
                return newShoppingList.ListName;
            else
                throw new CommandException("Not saved to database");
        }

        public ShoppingList Get(long chatId)
        {
            var shoppingList = _shoppingListRepository.GetByUserId(chatId);
            if (shoppingList == null)
            {
                throw new CommandException("Not found shopping list");
            }
            return shoppingList;
        }

        public void Update(ShoppingList shoppingList)
        {
            _shoppingListRepository.Update(shoppingList);
            if (_shoppingListRepository.Save() == 0)
                throw new CommandException("Not saved to database");
        }

        public string Clear(long chatId)
        {
            var shoppingList = Get(chatId);
            if (shoppingList.Items.Count == 0)
                throw new CommandException("Shopping list already clean");

            _itemRepository.DeleteRange(shoppingList.Items);
            if (_itemRepository.Save() > 0)
                return shoppingList.ListName;
            else
                throw new CommandException("Not saved to database");
        }

        public string AddItem(long chatId, string itemName)
        {
            var shoppingList = Get(chatId);
            Item item = new()
            {
                ShoppingList = shoppingList,
                ItemName = itemName
            };
            _itemRepository.Add(item);
            if (_itemRepository.Save() > 0)
                return item.ItemName;
            else
                throw new CommandException("Not saved to database");
        }

        public string BoughtAllItems(long chatId)
        {
            var shoppingList = Get(chatId);
            var items = shoppingList.Items;
            if (items.Count == 0)
            {
                throw new CommandException("Shopping list is empty");
            }
            foreach (var item in items)
            {
                item.IsBought = true;
                _itemRepository.Update(item);
            }
            if (_itemRepository.Save() > 0)
                return shoppingList.ListName;
            else
                throw new CommandException("Not saved to database");
        }

        public string ByuItem(long chatId, int itemNumber)
        {
            var shoppingList = Get(chatId);
            var itemsCount = shoppingList.Items.Count;
            if (itemsCount == 0)
            {
                throw new CommandException("Shopping list is empty");
            }
            else if (itemNumber <= 0 || itemNumber > itemsCount)
            {
                throw new CommandException($"Incorrect index. Enter number between 1 and {itemsCount}");
            }
            var item = shoppingList.Items[itemNumber - 1];
            item.IsBought = true;
            _itemRepository.Update(item);
            if (_itemRepository.Save() > 0)
                return shoppingList.ListName;
            else
                throw new CommandException("Not saved to database");
        }

        public string DeleteItem(long chatId, int itemNumber)
        {
            var shoppingList = Get(chatId);
            var itemsCount = shoppingList.Items.Count;
            if (itemsCount == 0)
            {
                throw new CommandException("Shopping list is empty");
            }
            else if (itemNumber <= 0 || itemNumber > itemsCount)
            {
                throw new CommandException($"Incorrect index. Enter number between 1 and {itemsCount}");
            }
            var item = shoppingList.Items[itemNumber - 1];
            _itemRepository.Delete(item);
            if (_itemRepository.Save() > 0)
                return shoppingList.ListName;
            else
                throw new CommandException("Not saved to database");
        }
    }
}
