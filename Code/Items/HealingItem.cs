using Godot;

public class HealingItem : Item
{
    public int HealingAmount;

    public HealingItem(Pickup source, Entity owner, int healingAmount) : base(source, owner)
    {
        HealingAmount = healingAmount;
    }

    public override void Use()
    {
        Owner.Heal(HealingAmount);
        base.Use();
    }
}