using Godot;
using System;

public class Player : KinematicBody2D
{
    [Signal] public delegate void PlayerMoved();
    [Signal] public delegate void PlayerDied();
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

        // If any input has been detected, try to move or attack
        if (direction.Length() != 0)
        {
            var collision = MoveAndCollide(direction, testOnly: true);

            if (collision?.Collider is Enemy enemy)
            {
                // We collided with an enemy, now we must fight
                Fight(enemy);
                EmitSignal(nameof(PlayerMoved));
            }
            else if (collision == null)
            {
                // No collision, move as normal
                // TODO: Move to common function 
                {
                    // No collision, move as normal
                    MoveAndCollide(direction);

                    // Sometimes the positions get off by ~.001 which breaks the pathing
                    // So if we divide the position by the tile size   (8.0001 = 1.000125), then round it (1.000125 = 1) and multiply it back (1 = 8)
                    // Then we can place the entity back on their tile
                    var newPos = Position / (float)TileScale;
                    Position = new Vector2(
                        Mathf.RoundToInt(newPos.x),
                        Mathf.RoundToInt(newPos.y)
                    ) * TileScale;
                }

                EmitSignal(nameof(PlayerMoved));
            }
        }
    }

    internal void Fight(Enemy enemy)
    {
        enemy.Damage(this.Attack);
    }

    public void Damage(int amount)
    {
        Health -= amount;
        GetParent().GetNode<HUD>("HUD").UpdateHealth(Health, MaxHealth);

        if (Health <= 0)
        {
            EmitSignal(nameof(PlayerDied));
            QueueFree();
        }
    }
}
