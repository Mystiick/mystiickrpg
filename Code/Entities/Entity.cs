using Godot;
using System;

public class Entity : KinematicBody2D
{
    public Inventory Inventory { get; } = new Inventory();
    public Equipment Equipment { get; } = new Equipment();
    public int MaxHealth { get; protected set; }
    public bool CanMove { get; set; }
    [Export] public int Health { get; protected set; }
    [Export] public int Attack { get; protected set; }
    [Export] public string LootTable { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Inventory.Owner = this;
    }

    public virtual void Heal(int amount)
    {
        Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
    }
}