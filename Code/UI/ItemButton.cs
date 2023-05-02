using Godot;

public partial class ItemButton : TextureButton
{
    [Signal] public delegate void ItemUsedEventHandler();
    [Signal] public delegate void ItemDroppedEventHandler(Item item);
    public Item Item;
    public Inventory Inventory;

    public void OnItemPressed()
    {
        if (Item != null && Item.Usable)
        {
            Inventory.UseItem(Item);
            EmitSignal(nameof(ItemUsed));
        }
    }

    public void OnItemDropped()
    {
        if (Item != null)
        {
            EmitSignal(nameof(ItemDropped), Item);
        }
    }

    public void OnItemGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton ie && ie.Pressed)
        {
            switch (ie.ButtonIndex)
            {
                case MouseButton.Left:
                    OnItemPressed();
                    break;

                case MouseButton.Right:
                    OnItemDropped();
                    break;
            }
        }
    }
}