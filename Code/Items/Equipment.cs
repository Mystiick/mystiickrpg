public class Equipment
{
    public Equipable Head { get; private set; }
    public Equipable Chest { get; private set; }
    public Equipable Legs { get; private set; }
    public Equipable Boots { get; private set; }
    public Equipable Shoulders { get; private set; }
    public Equipable Amulet { get; private set; }
    public Equipable RightHand { get; private set; }
    public Equipable LeftHand { get; private set; }

    /// <summary>
    /// Attempts to equip the specified item.
    /// </summary>
    /// <return>Any item that was unequiped in the process. Null if nothing was unequiped. The same item if it was not equiped.</returns>
    public Equipable EquipItem(Equipable item)
    {
        var output = Head;
        Head = item;

        return output;
    }
}