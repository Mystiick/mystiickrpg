using System.Collections.Generic;

public class Equipment
{
    public Equipable Head { get { return Items[Equipable.SlotType.Head]; } }
    public Equipable Chest { get { return Items[Equipable.SlotType.Chest]; } }
    public Equipable Legs { get { return Items[Equipable.SlotType.Legs]; } }
    public Equipable Boots { get { return Items[Equipable.SlotType.Boots]; } }
    public Equipable Shoulders { get { return Items[Equipable.SlotType.Shoulders]; } }
    public Equipable Amulet { get { return Items[Equipable.SlotType.Amulet]; } }
    public Equipable RightHand { get { return Items[Equipable.SlotType.RightHand]; } }
    public Equipable LeftHand { get { return Items[Equipable.SlotType.LeftHand]; } }

    public Dictionary<Equipable.SlotType, Equipable> Items;

    public Equipment()
    {
        Items = new Dictionary<Equipable.SlotType, Equipable>() {
            {Equipable.SlotType.Head, null},
            {Equipable.SlotType.Chest, null},
            {Equipable.SlotType.Legs, null},
            {Equipable.SlotType.Boots, null},
            {Equipable.SlotType.Shoulders, null},
            {Equipable.SlotType.Amulet, null},
            {Equipable.SlotType.RightHand, null},
            {Equipable.SlotType.LeftHand ,null}
        };
    }

    /// <summary>
    /// Attempts to equip the specified item.
    /// </summary>
    /// <return>Any item that was unequiped in the process. Null if nothing was unequiped. The same item if it was not equiped.</returns>
    public Equipable EquipItem(Equipable item)
    {
        Equipable output = Items[item.Slot];
        Items[item.Slot] = item;

        return output;
    }

}