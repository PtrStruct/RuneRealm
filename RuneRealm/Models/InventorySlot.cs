namespace RuneRealm.Models;

public class InventorySlot
{
    public int ItemId { get; set; }
    public int Amount { get; set; }
    public RSItem ToItem() => new() {ItemId = ItemId, Amount = Amount};
}