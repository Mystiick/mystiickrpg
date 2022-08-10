using Godot;
using System.Linq;

public class Player : Entity
{
    [Signal] public delegate void PlayerMoved();
    [Signal] public delegate void PlayerDied();
    [Export] public int TileScale = 8;
    [Export] public AudioStream[] Footsteps;
    [Export] public AudioStream DeathSound;

    private Main _main;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _main = GetNode<Main>("/root/Main");

        Reset();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        HandleInput();
    }

    /// <summary>
    /// Causes the player to deal damage to the specified enemy
    /// </summary>
    public void Fight(Enemy enemy)
    {
        enemy.Damage(this.Attack);
    }

    /// <summary>
    /// Damages the player for the specified amount
    /// </summary>
    public void Damage(int amount)
    {
        Health -= amount;
        _main.UserInterface.HUD.UpdateHealth(Health, MaxHealth);

        if (Health <= 0)
        {
            EmitSignal(nameof(PlayerDied));
            CanMove = false;
            Hide();
            PlaySound(DeathSound);
        }
    }

    /// <summary>
    /// Heals the player for the specified amount, up to the max health
    /// </summary>
    public override void Heal(int amount)
    {
        base.Heal(amount);
        _main.UserInterface.HUD.UpdateHealth(Health, MaxHealth);
    }

    /// <summary>
    /// Reserts the player's stats to a fresh copy
    /// </summary>
    public void Reset()
    {
        MaxHealth = 10;
        Health = 10;
        Attack = 1;
        CanMove = true;

        Inventory.Clear();
    }

    private void HandleInput()
    {
        if (CanMove && !_main.IsPaused)
        {
            Vector2 direction = Vector2.Zero;
            // Use elses here instead of checking each one individually to make sure only one direction is moved at a time
            // And 2 turns aren't taken by trying to move at an angle
            if (Input.IsActionPressed("move_up"))
                direction.y--;
            else if (Input.IsActionPressed("move_down"))
                direction.y++;
            else if (Input.IsActionPressed("move_left"))
                direction.x--;
            else if (Input.IsActionPressed("move_right"))
                direction.x++;

            direction *= TileScale;

            // If any input has been detected, try to move or attack
            if (direction.Length() != 0)
            {
                var collision = MoveAndCollide(direction, testOnly: true);

                if (collision == null)
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
                    PlaySound(Footsteps.Random());
                    FinishTurn();
                }
                else
                {
                    HandlePlayerCollision(collision);
                }
            }
            else if (Input.IsActionJustPressed("skip_turn"))
            {
                FinishTurn();
            }
        }
    }

    private void FinishTurn()
    {
        CanMove = false;
        EmitSignal(nameof(PlayerMoved));
    }

    private void HandlePlayerCollision(KinematicCollision2D collision)
    {
        if (collision.Collider is Enemy enemy)
        {
            // We collided with an enemy, now we must fight
            Fight(enemy);
            FinishTurn();
        }
        else if (collision.Collider is Door door)
        {
            if (door.State == Door.DoorState.Locked && this.Inventory.Any(x => x is Key))
            {
                this.Inventory.UseItem(this.Inventory.First(x => x is Key));

                door.Unlock();
                _main.UserInterface.HUD.UpdateHUD(this);
                FinishTurn();
            }
            else if (door.State == Door.DoorState.Closed)
            {
                door.Open();
                FinishTurn();
            }
        }
    }

    private void PlaySound(AudioStream sound)
    {
        var audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        audio.Stream = sound;
        audio.Play();
    }
}
