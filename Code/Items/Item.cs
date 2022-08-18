using Godot;
using System.Linq;
using System.Text;

public class Item : Godot.Object
{
    public int ID { get; set; }
    public string TexturePath { get; set; }
    public Texture Texture { get; set; }
    public string Name { get; set; }
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
        Name = source.Tooltip;
        Owner = owner;
    }

    public virtual bool Use() => true;

    public virtual Item Clone()
    {
        return new Item()
        {
            ID = this.ID,
            TexturePath = this.TexturePath,
            Texture = this.Texture,
            Name = this.Name,
            Tooltip = this.Tooltip,
            Usable = this.Usable,
            Modifiers = this.Modifiers?.Select(x => x.Clone()).ToArray()
        };
    }
    protected T CloneAs<T>() where T : Item, new()
    {
        return new T()
        {
            ID = this.ID,
            TexturePath = this.TexturePath,
            Texture = this.Texture,
            Name = this.Name,
            Tooltip = this.Tooltip,
            Usable = this.Usable,
            Modifiers = this.Modifiers?.Select(x => x.Clone()).ToArray()
        };
    }

    public virtual string BuildTooltip()
    {
        var output = new StringBuilder($"{Name}\n{Tooltip}");

        if (Modifiers?.Length > 0)
        {
            foreach (var stat in Modifiers)
            {
                output.Append("\n");
                output.Append(stat.Value > 0 ? "+" : "");
                output.Append(stat.Value);
                output.Append(" ");
                output.Append(stat.Type);
            }
        }

        return output.ToString();
    }
}