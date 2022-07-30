using Godot;
using System;

public class Pickup : Area2D
{
    [Signal] public delegate void ItemPickedUp(Pickup sender);
    public void OnPickupBodyEntered(PhysicsBody2D collision)
    {
        if (collision is Player p)
        {
            HandlePickup(p);
        }
    }

    public virtual void HandlePickup(Player player)
    {
        QueueFree();
        EmitSignal(nameof(ItemPickedUp), new[] { this });
    }
}
