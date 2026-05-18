namespace DetektivGame.Models;

public class Player
{
    public string Name { get; set; } = "Detektiiv";
    public int Score { get; private set; } = 0;
    public int Steps { get; private set; } = 0;
    public List<Item> Inventory { get; private set; } = new();

    public event Action? InventoryChanged;

    public bool AddToInventory(Item item)
    {
        if (Inventory.Count >= 8) return false;
        Inventory.Add(item);
        Score += 10;
        InventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveFromInventory(Item item)
    {
        bool removed = Inventory.Remove(item);
        if (removed) InventoryChanged?.Invoke();
        return removed;
    }

    public bool HasItem(string itemId) => Inventory.Any(i => i.Id == itemId);
    public Item? GetItem(string itemId) => Inventory.FirstOrDefault(i => i.Id == itemId);
    public void RemoveById(string itemId) { var item = GetItem(itemId); if (item != null) RemoveFromInventory(item); }
    public void AddStep() => Steps++;
    public void AddScore(int points) => Score += points;
}