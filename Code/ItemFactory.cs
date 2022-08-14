using Godot;
using System;
using System.Linq;

using Newtonsoft.Json;

public static class ItemFactory
{
    private static Item[] _allItems { get; set; }

    public static Item BuildItemByType(ItemType type, Texture texture, string tooltip, Pickup source, Entity owner)
    {
        switch (type)
        {
            case ItemType.Chicken:
                return new HealingItem(source, owner, ((Chicken)source).HealingAmount);

            case ItemType.Key:
                return new Key(source, owner) { Usable = false };

            case ItemType.Generic:
                return new Item(source, owner);

            default:
                throw new NotImplementedException($"ItemType of {type} is not yet implemented");
        }
    }

    public static void LoadItemsFromFile()
    {
        var items = new File();
        items.Open("res://Data/items.json", File.ModeFlags.Read);

        _allItems = JsonConvert.DeserializeObject<SerializedItem[]>(items.GetAsText()).Select(x => x.ToItem()).ToArray();
    }

    public static Item GetItemByID(int id)
    {
        var output = _allItems[id].Clone();
        output.Texture = ResourceLoader.Load<Texture>(output.TexturePath);

        return output;
    }
}


public enum ItemType
{
    Generic,
    Chicken,
    Key
}