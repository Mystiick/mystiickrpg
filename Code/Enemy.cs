using Godot;
using System;

public class Enemy : Node
{
    public int Health { get; private set; }
    public int Attack { get; private set; }
    [Export] EnemyType Type;

    public override void _Ready()
    {
        switch (Type)
        {
            case EnemyType.Crab:
                Health = 10;
                Attack = 0;
                break;

            case EnemyType.Zombie:
                Health = 2;
                Attack = 1;
                break;

            case EnemyType.Skeleton:
                Health = 4;
                Attack = 2;
                break;

            default: break;
        }
    }

    public void Damage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            QueueFree();
        }
    }

    public void Fight(Player player)
    {
        if (Health > 0)
        {
            player.Damage(Attack);
        }
    }

    enum EnemyType
    {
        Crab,
        Zombie,
        Skeleton
    }
}
