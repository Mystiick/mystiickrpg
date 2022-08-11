public class Equipable : Item
{
    public SlotType Slot;


    public enum SlotType
    {
        Head,
        Chest,
        Legs,
        Boots,
        Shoulders,
        Amulet,
        RightHand,
        LeftHand
    }
}