public class Key : Pickup
{
    public override void HandlePickup(Player player)
    {
        player.Keys++;
        base.HandlePickup(player);
    }
}
