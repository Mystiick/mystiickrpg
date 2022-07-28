using Godot;
using System;

public class Player : KinematicBody2D
{
    [Export] public int TileScale = 8;

    public int MaxHealth { get; private set; }
    public int Health { get; private set; }
    public int Shield { get; private set; }
    public int Attack { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MaxHealth = 6;
        Health = 6;
        Shield = 0;
        Attack = 1;

        GetParent().GetNode<HUD>("HUD").UpdateHealth(Health, MaxHealth);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        HandleInput();
    }

    internal void HandleInput()
    {
        Vector2 direction = Vector2.Zero;
        // Use elses here instead of checking each one individually to make sure only one direction is moved at a time
        // And 2 turns aren't taken by trying to move at an angle
        if (Input.IsActionJustPressed("move_up"))
            direction.y--;
        else if (Input.IsActionJustPressed("move_down"))
            direction.y++;
        else if (Input.IsActionJustPressed("move_left"))
            direction.x--;
        else if (Input.IsActionJustPressed("move_right"))
            direction.x++;

        direction *= TileScale;


        if (direction.Length() != 0)
        {
            var collision = MoveAndCollide(direction, testOnly: true);

            if (collision?.Collider is Enemy enemy)
            {
                // We collided with an enemy, now we must fight
                GD.Print("Collided with enemy");
                Fight(enemy);
            }
            else if (collision == null)
            {
                // No collision, move as normal
                MoveAndCollide(direction);

                Position = new Vector2(
                    Mathf.RoundToInt(Position.x),
                    Mathf.RoundToInt(Position.y)
                );
            }
        }
    }

    public void Fight(Enemy enemy)
    {
        enemy.Damage(this.Attack);
        enemy.Fight(this);
    }

    public void Damage(int amount)
    {
        Health -= amount;
        GetParent().GetNode<HUD>("HUD").UpdateHealth(Health, MaxHealth);

        if (Health <= 0)
        {
            QueueFree();
        }
    }

}
