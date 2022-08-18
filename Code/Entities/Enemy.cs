using Godot;
using System;
using System.Linq;

public class Enemy : Entity
{
    [Signal] public delegate void EnemyKilled(Enemy me);
    [Export] EnemyType Type;
    [Export] public int TileScale = 8;
    [Export] public Texture[] Bloodstains;
    [Export] public AudioStream[] HitSounds;
    [Export] public int Range { get; private set; }
    public bool CanSeePlayer { get; private set; }

    private Player player;

    public override void _Ready()
    {
        base._Ready();
        player = GetTree().Root.GetNode<Main>("/root/Main/")?.CurrentPlayer;
    }

    /// <summary>
    /// Runs every frame, sets a bool indicating if the current entity can see the player entity
    /// </summary>
    public override void _PhysicsProcess(float delta)
    {
        if (player != null)
        {
            Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;

            var result = spaceState.IntersectRay(this.Position + Vector2.One, player.Position + Vector2.One, new Godot.Collections.Array { this });
            if (result != null && result.Keys.Cast<string>().Contains("collider"))
            {
                CanSeePlayer = result["collider"] is Player;
            }
        }
    }

    /// <summary>
    /// Damages the enemy. If the enemy dies it is removed from the scene TODO: and drops are calculated if any exist.
    /// </summary>
    public void Damage(int amount)
    {
        Health -= amount;

        var audio = GetNode<AudioStreamPlayer>("/root/Main/EnemyHit");
        audio.Stream = HitSounds.Random();
        audio.Play();

        if (Health <= 0)
        {
            EmitSignal(nameof(EnemyKilled), this);
            QueueFree();

            DropStain();
            DropItem();
        }
    }

    /// <summary>
    /// Moves the enemy, and attacks them if they are within range
    /// </summary>
    public void TakeTurn()
    {
        if (!IsQueuedForDeletion() && Health > 0)
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

    /// <summary>
    /// Gets a normalized NSEW direction to the player. It only ever returns a cardinal direction, never NE, NW, SE, SW. 
    /// If the player is an equal distance vertically and horizontally from the unit, it will return a random direction of the two.
    /// </summary>
    private Vector2 GetDirectionToPlayer()
    {
        // Determine cardinal direction player is, in relation to the enemy
        Vector2 offset = (this.Position - player.Position) / (float)TileScale;
        offset = new Vector2(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y));

        Vector2 direction = Vector2.Zero;

        if (Math.Abs(offset.x) == Math.Abs(offset.y))
        {
            if (GD.Randi() % 2 == 0)
                direction.x += (offset.x < 0 ? 1 : -1);
            else
                direction.y += (offset.y < 0 ? 1 : -1);
        }
        else if (Math.Abs(offset.x) > Math.Abs(offset.y))
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

    private void DropStain()
    {
        // Place a randomized bloodstain on the ground and put it in the Environment layer
        var stain = new Sprite();
        stain.Texture = Bloodstains.Random();
        stain.Position = Position + new Vector2(4, 4);

        GetNode("%Environment").AddChild(stain);
    }

    private void DropItem()
    {
        if (!string.IsNullOrWhiteSpace(LootTable))
        {
            var drop = ItemFactory.GetTableByName(LootTable).GetDrop();

            if (drop != null)
            {
                GetNode<Main>("/root/Main").DropItem(drop, this.Position);
            }
        }
    }

    enum EnemyType
    {
        Crab,
        Zombie,
        Skeleton
    }
}
