using Godot;
using System;

public class Entity : KinematicBody2D
{
    public Inventory Inventory { get; } = new Inventory();
    public int MaxHealth { get; protected set; }
    [Export] public int Health { get; protected set; }
    [Export, Obsolete("Not yet implemented")] public int Shield { get; protected set; }
    [Export] public int Attack { get; protected set; }
    public bool CanMove { get; set; }

    public virtual void Heal(int amount)
    {
        Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
    }
}