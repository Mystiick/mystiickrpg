using Godot;

public class HealingItem : Item
{
    public int HealingAmount;

    public HealingItem(Pickup source, Entity owner, int healingAmount) : base(source, owner)
    {
        HealingAmount = healingAmount;
    }

    public override bool Use()
    {
        // Don't use the item if the owner is at full health
        if (Owner.Health >= Owner.MaxHealth)
            return false;

        Owner.Heal(HealingAmount);
        return true;
    }
}