using Godot;
using System.Linq;

public class Entity : KinematicBody2D
{
    public Inventory Inventory { get; } = new Inventory();
    public Equipment Equipment { get; } = new Equipment();
    public bool CanMove { get; set; }
    public int Health { get; protected set; }

    public int MaxHealth => BaseHealth + Equipment.ModifierByStat(Stat.StatType.Health).Sum(x => x.Value);
    public int MaxAttack => Attack + Equipment.ModifierByStat(Stat.StatType.Attack).Sum(x => x.Value);
    public int MaxDefense => BaseDefense + Equipment.ModifierByStat(Stat.StatType.Defense).Sum(x => x.Value);

    [Export] public int BaseDefense { get; protected set; } = 0;
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

    public virtual int RandomDefenseAmount()
    {
        // 10% chance to defend anything
        // TODO: Add range for higher defense = higher reduction chance
        if (MaxDefense > 0 && GD.Randi() % 100 >= 90)
        {
            int output = (int)(GD.Randi() % (MaxDefense + 1));
            return output;
        }
        else
        {
            return 0;
        }

    }
}