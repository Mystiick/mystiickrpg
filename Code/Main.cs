using Godot;

using System.Collections.Generic;
using System.Linq;

public class Main : Node
{
    public bool IsPaused;
    public UIManager UserInterface;

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
        base._Ready();

        // Read json files and prepopulate ItemFactory data
        ItemFactory.LoadItemsFromFile();
        ItemFactory.LoadLootTablesFromFile();

        _worldPrefix = "StarterDungeon/";

        UserInterface = GetNode<UIManager>("UIManager");
        UserInterface.HideAll();
        UserInterface.MainMenu.CallDeferred("show");

        CurrentPlayer = GetNode<Player>("%Player");
        CurrentPlayer.Hide();

        LoadSettings();

        _enemyTurns = new Queue<Enemy>();
        _enemyMove = new Timeout(.1f);
        _playerMove = new Timeout(.1f);
        IsPaused = true;
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("debug"))
        {
            UserInterface.Debug.ShowLevelSelect();
        }

        if (Input.IsActionJustPressed("pause_game"))
        {
            if (IsPaused)
            {
                IsPaused = false;
                UserInterface.Paused.Hide();
            }
            else
            {
                IsPaused = true;
                UserInterface.Paused.Show();
                UserInterface.Paused.GetChild<Control>(0).GetNode<Label>("YouDied").Hide();
            }
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
        if (ResourceLoader.Exists($"res://Maps/{_worldPrefix}{map}.tscn"))
        {
            UnloadCurrentMap();

            Node scene = ResourceLoader.Load<PackedScene>($"res://Maps/{_worldPrefix}{map}.tscn").Instance();
            _loadedScene = scene;
            _loadedScene.Connect(nameof(Level.LevelLoaded), this, "OnLevelLoaded");

            GetNode("GameContainer").GetNode("GameCam").CallDeferred("add_child", scene);
        }
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

    private void LoadSettings()
    {
        var settings = UserInterface.Settings.CurrentSettings;

        // Update video settings
        OS.WindowMaximized = settings.Window == Settings.WindowType.Maximized;
        OS.WindowFullscreen = settings.Window == Settings.WindowType.BorderlessWindowed;

        Engine.TargetFps = settings.MaxFps;

        // Update Sound settings
        foreach (AudioStreamPlayer a in GetTree().GetNodesInGroup("background_noise"))
            a.VolumeDb = GD.Linear2Db(settings.BackgroundVolume * settings.MasterVolume);

        foreach (AudioStreamPlayer a in GetTree().GetNodesInGroup("sound_effects"))
            a.VolumeDb = GD.Linear2Db(settings.SoundEffectsVolume * settings.MasterVolume);

        // Start the background noise if it's not already running.
        // It is not running by default to prevent it from playing a loud sound on startup when the user has turned it down before
        if (!GetNode<AudioStreamPlayer>("BackgroundNoise").Playing)
            GetNode<AudioStreamPlayer>("BackgroundNoise").Play();
    }

    #region | UI Events |

    private void OnMainMenuStartButtonPressed() => RestartGame();
    private void OnLoadLevel(string level) => LoadMap(level);
    private void OnPausedRetryPressed() => RestartGame();
    private void OnSettingsUpdated() => LoadSettings();

    private void RestartGame()
    {
        CurrentPlayer.Reset();
        CurrentPlayer.Show();

        UserInterface.HUD.UpdateHUD(CurrentPlayer);

        LoadMap("Level1");
        IsPaused = false;
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
        UserInterface.Paused.Show();
        UserInterface.Paused.GetNode<Label>("Control/YouDied").Show();
        UserInterface.Paused.UpdateDeathStats(CurrentPlayer);
        IsPaused = true;
    }

    /// <summary>
    /// Handles the event after the player has picked up an item.
    /// </summary>
    private void OnItemPickedUp(Pickup item)
    {
        UserInterface.HUD.UpdateHUD(CurrentPlayer);
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
        // TODO: Add counter
    }

    #endregion

}
