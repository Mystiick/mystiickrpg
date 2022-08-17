using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

public static class ItemFactory
{
    private static Item[] _allItems;
    private static Dictionary<string, DropTable> _dropTables;

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

    /// <summary>Builds a usable typed item by the given ID</summary>
    public static Item GetItemByID(int id)
    {
        var output = _allItems[id].Clone();
        output.Texture = ResourceLoader.Load<Texture>(output.TexturePath);

        return output;
    }

    /// <summary>Returns a full loot table by name</summary>
    public static DropTable GetTableByName(string name)
    {
        return _dropTables[name];
    }

    /// <summary>Reads the items.json data file and populates a cache by ID</summary>
    public static void LoadItemsFromFile()
    {
        var itemFile = new File();
        itemFile.Open("res://Data/items.json", File.ModeFlags.Read);

        var items = JsonConvert.DeserializeObject<SerializedItem[]>(itemFile.GetAsText()).Select(x => x.ToItem());
        _allItems = new Item[items.Max(x => x.ID) + 1];
        foreach (var item in items)
        {
            _allItems[item.ID] = item;
        }
    }

    public static void LoadLootTablesFromFile()
    {
        // Load from file
        var file = new File();
        file.Open("res://Data/drop_tables.json", File.ModeFlags.Read);

        // Process tables into dictionary of <Name, Table>
        var tables = JsonConvert.DeserializeObject<DropTable[]>(file.GetAsText());
        _dropTables = new Dictionary<string, DropTable>();
        foreach (DropTable table in tables)
        {
            _dropTables.Add(table.Name, table);
        }
    }
}


public enum ItemType
{
    Generic,
    Chicken,
    Key
}