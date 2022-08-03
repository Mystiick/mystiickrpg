using Godot;

public class ItemButton : TextureButton
{
    [Signal] public delegate void ItemUsed();
    public Item Item;
    public Inventory Inventory;

    public void OnItemPressed()
    {
        Inventory.UseItem(Item);
        EmitSignal(nameof(ItemUsed));
    }
}