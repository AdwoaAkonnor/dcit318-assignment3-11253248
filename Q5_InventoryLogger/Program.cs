using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DCIT318_Q5
{
    // Marker interface
    public interface IInventoryEntity { int Id { get; } }

    // Immutable inventory item as record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // Generic logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item) => _log.Add(item);
        public List<T> GetAll() => new List<T>(_log);

        public void SaveToFile()
        {
            var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void LoadFromFile()
        {
            if (!File.Exists(_filePath)) return;
            var json = File.ReadAllText(_filePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);
            _log.Clear();
            if (items != null) _log.AddRange(items);
        }
    }

    // App wrapper
    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath) { _logger = new InventoryLogger<InventoryItem>(filePath); }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Pen", 100, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Notebook", 50, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Marker", 25, DateTime.Now));
        }

        public void SaveData() => _logger.SaveToFile();
        public void LoadData() => _logger.LoadFromFile();
        public void PrintAllItems()
        {
            foreach (var it in _logger.GetAll()) Console.WriteLine(it);
        }
    }

    class Program
    {
        static void Main()
        {
            var filePath = "inventory_log.json";
            var app = new InventoryApp(filePath);

            // Seed and save
            app.SeedSampleData();
            app.SaveData();
            Console.WriteLine("Saved inventory to " + filePath);

            // Simulate new session: create a new app instance and load
            var newApp = new InventoryApp(filePath);
            newApp.LoadData();
            Console.WriteLine("Loaded items:");
            newApp.PrintAllItems();
        }
    }
}
