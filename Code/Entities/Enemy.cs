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
    private Player player;

    public override void _Ready()
    {
        player = GetTree().Root.GetNode<Player>("Main/Player");

        switch (Type)
        {
            case EnemyType.Crab:
                Health = 15;
                Attack = 2;
                Range = 1;
                break;

            case EnemyType.Zombie:
                Health = 2;
                Attack = 1;
                Range = 1;
                break;

            case EnemyType.Skeleton:
                Health = 3;
                Attack = 2;
                Range = 1;
                break;

            default: break;
        }
    }

    /// <summary>
    /// Runs every frame, sets a bool indicating if the current entity can see the player entity
    /// </summary>
    public override void _PhysicsProcess(float delta)
    {
        Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;

        var result = spaceState.IntersectRay(this.Position + Vector2.One, player.Position + Vector2.One, new Godot.Collections.Array { this });
        if (result != null && result.Keys.Cast<string>().Contains("collider"))
        {
            CanSeePlayer = result["collider"] is Player;
        }
    }

    /// <summary>
    /// Damages the enemy. If the enemy dies it is removed from the scene TODO: and drops are calculated if any exist.
    /// </summary>
    public void Damage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            QueueFree();
        }
    }

    /// <summary>
    /// Causes the enemy to attack the player TODO: and play the assiociated action
    /// </summary>
    public void Fight()
    {
        if (Health > 0)
        {
            player.Damage(Attack);
        }
    }

    /// <summary>
    /// Moves the enemy, and attacks them if they are within range
    /// </summary>
    public void TakeTurn()
    {
        if (Health > 0)
        {
            Vector2 directionToPlayer = GetDirectionToPlayer();

            // If the enemy can see the player, move toward them or attack them if they're in range
            if (CanSeePlayer)
            {
                if (PlayerIsInRange())
                {
                    player.Damage(Attack);
                }
                else
                {
                    MoveTowardPosition(directionToPlayer);
                }
            }
        }
    }

    /// <summary>
    /// Checks if the player is within range vertically or horizontally, but not diagonally 
    /// </summary>
    private bool PlayerIsInRange()
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

    /// <summary>
    /// Moves the enemy toward the player 1 tile
    /// </summary>
    private void MoveTowardPosition(Vector2 direction)
    {
        if (MoveAndCollide(direction, testOnly: true) == null)
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

    private Vector2 GetDirectionToPlayer()
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
