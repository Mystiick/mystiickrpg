public partial class Equipable : Item
{
    public SlotType Slot { get; set; }

    public override Item Clone()
    {
        Equipable output = base.CloneAs<Equipable>();
        output.Slot = this.Slot;

        return output;
    }

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