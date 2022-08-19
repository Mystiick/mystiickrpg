using Godot;
using System.Linq;

public class Entity : KinematicBody2D
{
    public Inventory Inventory { get; } = new Inventory();
    public Equipment Equipment { get; } = new Equipment();
    public bool CanMove { get; set; }
    public int Health { get; protected set; }
    public int MaxHealth
    {
        get => BaseHealth + Equipment.Items.
                                Where(x => x.Value != null).
                                SelectMany(x => x.Value?.Modifiers).
                                Where(x => x.Type == Stat.StatType.Health).
                                Sum(x => x.Value);
    }

    [Export] public int BaseHealth { get; protected set; }
    [Export] public int Attack { get; protected set; }
    [Export] public string LootTable { get; set; } = "basic_dungeon";

    public override void _Ready()
    {
        base._Ready();
        Inventory.Owner = this;
        Health = MaxHealth;
    }

    public virtual void Heal(int amount)
    {
        Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
    }
}