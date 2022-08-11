using Godot;

public class Item
{
    public Texture Texture { get; set; }
    public string Tooltip { get; set; }
    public Entity Owner { get; set; }
    public bool Usable { get; set; } = true;

    public Item()
    {
    }

    public Item(Pickup source, Entity owner)
    {
        Texture = source.GetNode<Sprite>("Sprite").Texture;
        Tooltip = source.Tooltip;
        Owner = owner;
    }

    public virtual bool Use() => true;
}