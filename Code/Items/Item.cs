using Godot;
using System.Linq;
using System.Text;

public class Item
{
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

public class SerializedItem : Item
{
    public string Type { get; set; }
    // Equipable properties
    public Equipable.SlotType Slot;

    // HealintItem properties
    public int HealingAmount;

    public T ToItem<T>() where T : Item
    {
        return (T)(object)ToItem();
    }

    public Item ToItem()
    {
        switch (Type?.ToLower())
        {
            case "equipable":
                return new Equipable()
                {
                    Slot = this.Slot,
                    TexturePath = this.TexturePath,
                    Modifiers = this.Modifiers,
                    Texture = this.Texture,
                    Name = this.Name,
                    Tooltip = this.Tooltip,
                    Usable = this.Usable
                };
        }

        return this.Clone();
    }
}