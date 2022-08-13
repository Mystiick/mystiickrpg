using Godot;
using System.Linq;

public class Item
{
    public string TexturePath { get; set; }
    public Texture Texture { get; set; }
    public string Tooltip { get; set; }
    public Entity Owner { get; set; }
    public bool Usable { get; set; } = true;
    public Stat[] Modifiers { get; set; }

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

    public virtual Item Clone()
    {
        return new Item()
        {
            TexturePath = this.TexturePath,
            Texture = this.Texture,
            Tooltip = this.Tooltip,
            Usable = this.Usable,
            Modifiers = this.Modifiers.Select(x => x.Clone()).ToArray()
        };
    }
}
