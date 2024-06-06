using RuneRealm.Cache.Items;
using RuneRealm.Constants;
using RuneRealm.Entities;
using RuneRealm.Models;

namespace RuneRealm.Managers;

public class InventoryManager
{
    private readonly Player _player;
    public InventorySlot[] Inventory { get; set; }

    public InventoryManager()
    {
        
    }
    
    public InventoryManager(Player player)
    {
        _player = player;

        Inventory = Enumerable.Repeat(new InventorySlot { ItemId = -1, Amount = 0 }, 28).ToArray();

        Inventory[0] = new InventorySlot { ItemId = 1277, Amount = 1 };
        Inventory[1] = new InventorySlot { ItemId = 1171, Amount = 1 };
        Inventory[2] = new InventorySlot { ItemId = 841, Amount = 1 };
        Inventory[3] = new InventorySlot { ItemId = 882, Amount = 50 };
        Inventory[4] = new InventorySlot { ItemId = 556, Amount = 50 };
        Inventory[5] = new InventorySlot { ItemId = 558, Amount = 50 };
        
        
        Inventory[6] = new InventorySlot { ItemId = 995, Amount = 25 };
    }

    public int GetEmptySlotCount()
    {
        return Inventory.Count(x => x.ItemId == -1);
    }

    public bool HasFreeSlot(int itemId)
    {
        if (Inventory.Any(slot => slot.ItemId == itemId || slot.ItemId == -1))
            return true;

        _player.PacketBuilder.SendMessage("Not enough inventory space.");
        return false;
    }

    public int AddItem(int itemId, int amount = 1)
    {
        var itemDefinition = ItemDefinition.Lookup(itemId);
        if (itemDefinition == null)
            return -1;

        var stackable = itemDefinition.Stackable;
        var noted = itemDefinition.IsNote();
        if (stackable || noted)
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i].ItemId == itemId)
                {
                    Inventory[i].Amount += amount;
                    return i;
                }
            }
        }

        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i].ItemId != -1) continue;

            Inventory[i] = new InventorySlot
            {
                ItemId = itemId,
                Amount = amount
            };

            return i;
        }

        return -1;
    }

    public void RemoveAtIndex(int index)
    {
        Inventory[index] = new InventorySlot { ItemId = -1, Amount = 0 };
    }

    public int FindFirstAvailableSlot(int itemId = -1)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i].ItemId == itemId || Inventory[i].ItemId == -1)
                return i;
        }

        return -1;
    }

    public void Clear()
    {
        for (var i = 0; i < Inventory.Length; i++)
            Inventory[i] = new InventorySlot { ItemId = -1, Amount = 0 };
    }

    public void Refresh()
    {
        for (var i = 0; i < Inventory.Length; i++)
        {
            var slot = Inventory[i];
            _player.PacketBuilder.UpdateSlot(i, slot.ItemId, slot.Amount, GameInterfaces.DefaultInventory);
        }
    }
}