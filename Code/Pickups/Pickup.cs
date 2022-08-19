using Godot;
using System;

public class Pickup : Area2D
{
    [Signal] public delegate void ItemPickedUp(Pickup sender);
    [Export] public AudioStream[] PickupSounds;
    [Export] public string ItemName;
    [Export(PropertyHint.MultilineText)] public string Tooltip;
    [Export] public ItemType Type;

    public Item Item { get; set; }

    public override void _Ready()
    {
        base._Ready();
        this.CallDeferred(nameof(ConnectBody));
    }

    public void ConnectBody()
    {
        Connect("body_entered", this, nameof(OnPickupBodyEntered));
    }

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
        if (player.Inventory.HasSpace)
        {
            if (PickupSounds != null)
            {
                var audio = GetNode<AudioStreamPlayer>("/root/Main/Pickup");
                audio.Stream = PickupSounds.Random();
                audio.Play();
            }

            if (Item != null)
            {
                player.Inventory.Add(Item);
            }
            else
            {
                Sprite s = GetNode<Sprite>("Sprite");
                player.Inventory.Add(ItemFactory.BuildItemByType(Type, s.Texture, $"{ItemName}\n{Tooltip}", this, player));
            }

            QueueFree();
            EmitSignal(nameof(ItemPickedUp), this);
        }
    }
}
