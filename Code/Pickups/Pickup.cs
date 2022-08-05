using Godot;
using System;

public class Pickup : Area2D
{
    [Signal] public delegate void ItemPickedUp(Pickup sender);
    [Export] public AudioStream[] PickupSounds;
    [Export] public string ItemName;
    [Export(PropertyHint.MultilineText)] public string Tooltip;
    [Export] public ItemType Type;

    public void OnPickupBodyEntered(PhysicsBody2D collision)
    {
        if (collision is Player p)
        {
            HandlePickup(p);
        }
    }

    /// <summary>
    /// If the player's inventory has space, picks ups the item, plays any associated sounds, and adds the item to the inventory.
    /// Frees the pickup after it has been picked
    /// </summary>
    public virtual void HandlePickup(Player player)
    {
        if (player.Inventory.HasSpace())
        {
            var audio = GetNode<AudioStreamPlayer>("/root/Main/Pickup");
            audio.Stream = PickupSounds.Random();
            audio.Play();

            Sprite s = GetNode<Sprite>("Sprite");
            player.Inventory.Add(BuildItemByType(Type, s.Texture, $"{ItemName}\n{Tooltip}", player));

            QueueFree();
            EmitSignal(nameof(ItemPickedUp), this);
        }
    }

    private Item BuildItemByType(ItemType type, Texture texture, string tooltip, Entity owner)
    {
        switch (type)
        {
            case ItemType.Chicken:
                return new HealingItem(this, owner, ((Chicken)this).HealingAmount);

            case ItemType.Key:
                return new Key(this, owner);

            case ItemType.Generic:
                return new Item(this, owner);

            default:
                throw new NotImplementedException($"ItemType of {type} is not yet implemented");
        }
    }

    public enum ItemType
    {
        Generic,
        Chicken,
        Key
    }
}
