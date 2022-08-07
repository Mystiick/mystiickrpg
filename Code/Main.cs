using Godot;

using System.Collections.Generic;
using System.Linq;

public class Main : Node
{
    private Node _loadedScene;
    private string _worldPrefix;
    private Queue<Enemy> _enemyTurns;
    private Timeout _enemyMove;
    private Timeout _playerMove;
    private Player _player;

    public Player CurrentPlayer
    {
        get
        {
            return _player ?? GetNode<Player>("%Player");
        }
        private set
        {
            _player = value;
        }
    }

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        //OS.WindowMaximized = true;
        _worldPrefix = "StarterDungeon/";

        GetNode<HUD>("HUD").GetChild<Control>(0).Hide();
        GetNode<YouDied>("YouDied").GetChild<Control>(0).Hide();
        CurrentPlayer = GetNode<Player>("%Player");
        CurrentPlayer.Hide();

        _enemyTurns = new Queue<Enemy>();
        _enemyMove = new Timeout(.1f);
        _playerMove = new Timeout(.1f);
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("debug"))
        {
            GetNode<DebugUI>("DebugUI").ShowLevelSelect();
        }

        HandleTimers(delta);
    }

    /// <summary>
    /// Handles the player and enemy turn timers to give some delay to player and enemy movement.
    /// </summary>
    private void HandleTimers(float delta)
    {
        if (_enemyTurns.Any())
        {
            _enemyMove.Process(delta);

            if (_enemyMove.Elapsed)
            {
                _enemyMove.Reset();
                var enemy = _enemyTurns.Dequeue();
                if (IsInstanceValid(enemy))
                    enemy.TakeTurn();
            }
        }
        else
        {
            _playerMove.Process(delta);

            if (_playerMove.Elapsed && CurrentPlayer.Health > 0)
            {
                CurrentPlayer.CanMove = true;
            }
        }
    }

    /// <summary>
    /// Loads the specified map by name, moves the player to the spawn location, and moves the map to the top of the node tree (right under the background).
    /// </summary>
    private void LoadMap(string map)
    {
        UnloadCurrentMap();

        Node scene = ResourceLoader.Load<PackedScene>($"res://Maps/{_worldPrefix}{map}.tscn").Instance();
        _loadedScene = scene;
        _loadedScene.Connect(nameof(Level.LevelLoaded), this, "OnLevelLoaded");

        GetNode("GameContainer").GetNode("GameCam").CallDeferred("add_child", scene);
    }

    private void UnloadCurrentMap()
    {
        // Free existing objects we're listening to for events
        // If we don't do this, calling `GetNodesInGroup` will get these dying objects during OnLevelLoaded, since they might not have been clenaed up yet
        List<Node> entities = new List<Node>();
        entities.AddRange(GetTree().GetNodesInGroup("pickups").Cast<Node>());
        entities.AddRange(GetTree().GetNodesInGroup("stairs").Cast<Node>());
        entities.AddRange(GetTree().GetNodesInGroup("enemies").Cast<Node>());

        foreach (Node s in entities)
        {
            s.QueueFree();
        }

        // Free the entire scene
        if (_loadedScene != null)
            _loadedScene.QueueFree();
    }

    /// <summary>
    /// Called once the level has finished loading and _Ready has been called on that level
    /// </summary>
    private void OnLevelLoaded(Level sender)
    {
        // Move the level up to the top level, so it doesn't draw over the player/enemies
        GetNode("/root/Main/GameContainer/GameCam").MoveChild(sender, 0);

        // Move the player to the spawn point
        Position2D playerSpawn = sender.GetNode<Position2D>("PlayerSpawn");
        CurrentPlayer.Position = playerSpawn.Position;

        // Listen to events for pickups and stairs and enemies
        Godot.Collections.Array pickups = GetTree().GetNodesInGroup("pickups");
        pickups.ConnectAll(nameof(Pickup.ItemPickedUp), this, nameof(OnItemPickedUp));

        Godot.Collections.Array stairs = GetTree().GetNodesInGroup("stairs");
        stairs.ConnectAll(nameof(Stairs.StairsEntered), this, nameof(OnStairsEntered));

        Godot.Collections.Array enemies = GetTree().GetNodesInGroup("enemies");
        enemies.ConnectAll(nameof(Enemy.EnemyKilled), this, nameof(OnEnemyKilled));
    }

    #region | UI Events |

    private void OnMainMenuStartButtonPressed()
    {
        RestartGame();
    }

    private void OnDebugLoadLevelPressed(string level)
    {
        switch (level.ToLower())
        {
            case "genji":
                _player.Heal(_player.MaxHealth);
                break;
            case "thisisfine":
                foreach (var light in GetTree().GetNodesInGroup("lights").Cast<Light2D>()) { light.Enabled = true; }
                break;
            default:
                LoadMap(level);
                break;
        }
    }

    private void OnYouDiedRetryPressed()
    {
        RestartGame();
    }

    private void RestartGame()
    {
        GetNode<YouDied>("YouDied").GetChild<Control>(0).Hide();
        GetNode<MainMenu>("MainMenu").GetChild<Control>(0).Hide();
        GetNode<HUD>("HUD").GetChild<Control>(0).Show();

        CurrentPlayer.Reset();
        CurrentPlayer.Show();

        LoadMap("Level1");
    }

    #endregion

    #region | Game Events |
    /// <summary>
    /// Handles the event after the player has moved. Updates enemies and allows them to move/attack.
    /// </summary>
    private void OnPlayerMoved()
    {
        _enemyTurns.Clear();
        _playerMove.Reset();

        Godot.Collections.Array enemies = GetTree().GetNodesInGroup("enemies");
        foreach (Enemy e in enemies)
        {
            if (e.CanSeePlayer)
                _enemyTurns.Enqueue(e);
        }
    }

    /// <summary>
    /// Shows the "You Died" UI with stats of the player's adventure
    /// </summary>
    private void OnPlayerDied()
    {
        var ui = GetNode<YouDied>("YouDied");
        ui.GetChild<Control>(0).Show();
        ui.UpdateDeathStats(CurrentPlayer);
    }

    /// <summary>
    /// Handles the event after the player has picked up an item.
    /// </summary>
    private void OnItemPickedUp(Pickup item)
    {
        HUD hud = GetNode<HUD>("/root/Main/HUD");
        hud.UpdateHUD(CurrentPlayer);
    }

    /// <summary>
    /// Moves the player to the level specified in the stairs' target
    /// </summary>
    private void OnStairsEntered(Stairs stairs)
    {
        LoadMap(stairs.Destination);
    }

    private void OnEnemyKilled(Enemy enemy)
    {
        // Place a randomized bloodstain on the ground and put it in the Environment layer
        var stain = new Sprite();
        stain.Texture = enemy.Bloodstains.Random();
        stain.Position = enemy.Position + new Vector2(4, 4);
        _loadedScene.GetNode<Node>("Environment").AddChild(stain);
    }

    #endregion

}
