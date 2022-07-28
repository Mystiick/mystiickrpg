using Godot;
using System;
using System.Linq;

public class Enemy : KinematicBody2D
{
    [Export] EnemyType Type;
    [Export] public int TileScale = 8;
    public int Health { get; private set; }
    public int Attack { get; private set; }
    public int Range { get; private set; }

    private bool CanSeePlayer;

    public override void _Ready()
    {
        switch (Type)
        {
            case EnemyType.Crab:
                Health = 10;
                Attack = 0;
                Range = 1;
                break;

            case EnemyType.Zombie:
                Health = 2;
                Attack = 1;
                Range = 1;
                break;

            case EnemyType.Skeleton:
                Health = 4;
                Attack = 2;
                Range = 1;
                break;

            default: break;
        }
    }
    public override void _PhysicsProcess(float delta)
    {
        Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
        Player player = GetTree().Root.GetNode<Player>("Main/Player");

        var result = spaceState.IntersectRay(this.Position + Vector2.One, player.Position + Vector2.One, new Godot.Collections.Array { this });

        if (result != null && result.Keys.Cast<string>().Contains("collider"))
        {
            CanSeePlayer = result["collider"] is Player;
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

    public void TakeTurn()
    {
        if (Health > 0)
        {
            Player player = GetTree().Root.GetNode<Player>("Main/Player");
            Vector2 directionToPlayer = GetDirectionToPlayer(player);

            // If the enemy can see the player, move toward them or attack them if they're in range
            if (CanSeePlayer)
            {
                if (PlayerIsInRange(player))
                {
                    player.Damage(Attack);
                }
                else
                {
                    MoveTowardPlayer(player, directionToPlayer);
                }
            }
        }
    }

    // Check if the player is within range to be attacked
    private bool PlayerIsInRange(Player player)
    {
        Vector2 offset = player.Position - this.Position;

        if (Math.Abs(offset.x) <= Range * TileScale && offset.y == 0)
        {
            return true;
        }
        else if (Math.Abs(offset.y) <= Range * TileScale && offset.x == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Moves the enemy toward the player 1 tile
    private void MoveTowardPlayer(Player player, Vector2 direction)
    {
        var collision = MoveAndCollide(direction, testOnly: true);

        if (collision == null)
        {
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
        }
    }

    private Vector2 GetDirectionToPlayer(Player player)
    {
        // Determine cardinal direction player is, in relation to the enemy
        Vector2 offset = this.Position - player.Position;
        Vector2 direction = Vector2.Zero;

        if (Math.Abs(offset.x) > Math.Abs(offset.y))
        {
            // More distance to move horizontal, so try to move that way first
            direction.x += (offset.x < 0 ? 1 : -1);
        }
        else
        {
            // More vertical distance, go that way
            direction.y += (offset.y < 0 ? 1 : -1);
        }

        // Move that direction 1 tile
        direction *= TileScale;

        return direction;
    }

    enum EnemyType
    {
        Crab,
        Zombie,
        Skeleton
    }
}
