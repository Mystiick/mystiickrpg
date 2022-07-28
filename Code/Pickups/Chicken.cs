using Godot;

public class Chicken : Pickup
{
    [Export] public int HealingAmount = 6;

    public override void HandlePickup(Player player)
    {
        player.Heal(HealingAmount);
        base.HandlePickup(player);
    }
}
