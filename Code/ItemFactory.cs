using Godot;
using System;

public static class ItemFactory
{
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

}

public enum ItemType
{
    Generic,
    Chicken,
    Key
}