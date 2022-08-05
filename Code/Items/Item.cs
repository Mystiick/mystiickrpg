using Godot;

public class Item
{
    public Texture Texture { get; }
    public string Tooltip { get; }
    public Entity Owner { get; set; }
    public bool Usable { get; set; }

    public Item()
    {
    }

    public Item(Pickup source, Entity owner)
    {
        Texture = source.GetNode<Sprite>("Sprite").Texture;
        Tooltip = source.Tooltip;
        Owner = owner;
    }

    public virtual void Use()
    {

    }
}