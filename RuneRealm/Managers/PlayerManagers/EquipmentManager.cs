using RuneRealm.Constants;
using RuneRealm.Entities;
using RuneRealm.Models;

namespace RuneRealm.Managers;

public class EquipmentManager
{
    private readonly Player _player;

    
    public EquipmentManager(Player player)
    {
        _player = player;
        Equipment = new Dictionary<EquipmentSlot, RSItem>
        {
            { EquipmentSlot.Helmet, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Cape, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Amulet, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Weapon, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Chest, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Shield, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Legs, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Gloves, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Boots, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Ring, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Ammo, new RSItem { ItemId = -1, Amount = 0 } }
        };
    }

    public Dictionary<EquipmentSlot, RSItem> Equipment { get; set; } = new();

    public void Equip(RSItem rsItem, EquipmentSlot slot)
    {
        if (Equipment.ContainsKey(slot))
            Unequip(slot);

        Equipment[slot] = rsItem;
    }

    public void Unequip(EquipmentSlot slot)
    {
        if (Equipment.ContainsKey(slot))
            Equipment[slot] = new RSItem
            {
                ItemId = -1,
                Amount = 0
            };
    }

    public RSItem GetItem(EquipmentSlot slot)
    {
        if (Equipment.ContainsKey(slot))
            return Equipment[slot];

        return null;
    }

    public bool HasBowEquipped()
    {
        if (GetItem(EquipmentSlot.Weapon) == null)
            return false;

        return Array.Exists(GameConstants.BOWS, i => i == GetItem(EquipmentSlot.Weapon).ItemId);
    }


    public EquipmentSlot GetEquipmentSlotByItemId(int itemID)
    {
        if (GameConstants.IsItemInArray(itemID, GameConstants.BOWS) ||
            GameConstants.IsItemInArray(itemID, GameConstants.OTHER_RANGE_WEAPONS))
            return EquipmentSlot.Weapon;
        if (GameConstants.IsItemInArray(itemID, GameConstants.ARROWS))
            return EquipmentSlot.Ammo;
        if (GameConstants.IsItemInArray(itemID, GameConstants.capes))
            return EquipmentSlot.Cape;
        if (GameConstants.IsItemInArray(itemID, GameConstants.boots))
            return EquipmentSlot.Boots;
        if (GameConstants.IsItemInArray(itemID, GameConstants.gloves))
            return EquipmentSlot.Gloves;
        if (GameConstants.IsItemInArray(itemID, GameConstants.shields))
            return EquipmentSlot.Shield;
        if (GameConstants.IsItemInArray(itemID, GameConstants.hats))
            return EquipmentSlot.Helmet;
        if (GameConstants.IsItemInArray(itemID, GameConstants.amulets))
            return EquipmentSlot.Amulet;
        if (GameConstants.IsItemInArray(itemID, GameConstants.rings))
            return EquipmentSlot.Ring;
        if (GameConstants.IsItemInArray(itemID, GameConstants.body))
            return EquipmentSlot.Chest;
        if (GameConstants.IsItemInArray(itemID, GameConstants.legs))
            return EquipmentSlot.Legs;
        // Default to Weapon if no match
        return EquipmentSlot.Weapon;
    }

    public void Clear()
    {
        Equipment = new Dictionary<EquipmentSlot, RSItem>
        {
            { EquipmentSlot.Helmet, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Cape, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Amulet, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Weapon, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Chest, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Shield, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Legs, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Gloves, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Boots, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Ring, new RSItem { ItemId = -1, Amount = 0 } },
            { EquipmentSlot.Ammo, new RSItem { ItemId = -1, Amount = 0 } }
        };
    }

     public void Refresh()
     {
         foreach (var entry in Equipment)
         {
             var equipmentSlot = entry.Key;
             var item = entry.Value;
             var itemId = item.ItemId;
             var amount = item.Amount;
             _player.PacketBuilder.UpdateSlot((int)equipmentSlot, itemId, amount, GameInterfaces.Equipment);
         }
    
         _player.Flags |= PlayerUpdateFlags.Appearance;
     }
}