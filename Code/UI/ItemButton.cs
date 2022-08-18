using Godot;

public class ItemButton : TextureButton
{
    [Signal] public delegate void ItemUsed();
    [Signal] public delegate void ItemDropped(Item item);
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
                case MouseButtons.Left:
                    OnItemPressed();
                    break;

                case MouseButtons.Right:
                    OnItemDropped();
                    break;
            }
        }
    }
}