// Program.cs (Question 3)
using System;
using System.Collections.Generic;

namespace DCIT318_Q3
{
    // Marker interface for items
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // Product types
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
        }

        public override string ToString() => $"{Name} (ID:{Id}) - Brand:{Brand}, Qty:{Quantity}, Warranty:{WarrantyMonths}m";
    }

    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
        }

        public override string ToString() => $"{Name} (ID:{Id}) - Qty:{Quantity}, Expires:{ExpiryDate:d}";
    }

    // Custom exceptions
    public class DuplicateItemException : Exception { public DuplicateItemException(string msg): base(msg){} }
    public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg): base(msg){} }
    public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg): base(msg){} }

    // Generic inventory repo
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id)) throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item)) throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id)) throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => new List<T>(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // Manager that uses repositories
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Headset", 50, "Sony", 6));

            _groceries.AddItem(new GroceryItem(10, "Rice", 100, DateTime.Now.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(11, "Beans", 80, DateTime.Now.AddMonths(8)));
            _groceries.AddItem(new GroceryItem(12, "Milk", 30, DateTime.Now.AddDays(7)));
        }

        public void PrintAllGroceryItems() => PrintAllItems(_groceries);
        public void PrintAllElectronicItems() => PrintAllItems(_electronics);

        private void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            Console.WriteLine($"=== {typeof(T).Name} items ===");
            foreach (var it in repo.GetAllItems()) Console.WriteLine(it);
        }

        // Demonstration helpers with try/catch
        public void TryAddDuplicateElectronic()
        {
            try
            {
                _electronics.AddItem(new ElectronicItem(1, "DuplicateLaptop", 5, "HP", 12));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected error (duplicate add): " + ex.Message);
            }
        }

        public void TryRemoveNonExistentGrocery()
        {
            try
            {
                _groceries.RemoveItem(999);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected error (remove non-existent): " + ex.Message);
            }
        }

        public void TryUpdateInvalidQuantity()
        {
            try
            {
                _groceries.UpdateQuantity(10, -5); // invalid
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected error (invalid quantity): " + ex.Message);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var manager = new WareHouseManager();
            manager.SeedData();
            manager.PrintAllGroceryItems();
            manager.PrintAllElectronicItems();

            Console.WriteLine("\nTesting exceptions:");
            manager.TryAddDuplicateElectronic();
            manager.TryRemoveNonExistentGrocery();
            manager.TryUpdateInvalidQuantity();
        }
    }
}
