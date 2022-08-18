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
                Equipable eq = this.CloneAs<Equipable>();
                eq.Slot = this.Slot;

                return eq;

            case "key":
                return this.CloneAs<Key>();

            case "healing":
                HealingItem hi = this.CloneAs<HealingItem>();
                hi.HealingAmount = this.HealingAmount;

                return hi;

            default:
                return this.Clone();

        }
    }
}