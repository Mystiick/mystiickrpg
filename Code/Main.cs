using Godot;

using System.Collections.Generic;
using System.Linq;

public partial class Main : Node
{
    public bool IsPaused;
    public UIManager UserInterface;

    private Level _loadedScene;
    private string _worldPrefix;
    private Queue<Enemy> _enemyTurns;
    private Timeout _enemyMove;
    private Timeout _playerMove;
    private Player _player;
    private DungeonManager _dungeonManager;

    protected DungeonManager DungeonManager => _dungeonManager ?? LoadDungeonManager();
    public Player CurrentPlayer
    {
        get
        {
            return _player ?? LoadPlayer();
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
        GD.Print("Ready");
        base._Ready();

        // Read json files and prepopulate ItemFactory data
        ItemFactory.LoadItemsFromFile();
        ItemFactory.LoadLootTablesFromFile();

        _worldPrefix = "StarterDungeon/";

        UserInterface = GetNode<UIManager>("UIManager");
        UserInterface.HideAll();
        UserInterface.MainMenu.CallDeferred("show");

        CurrentPlayer.Hide();

        LoadSettings();

        _enemyTurns = new Queue<Enemy>();
        _enemyMove = new Timeout(.1f);
        _playerMove = new Timeout(.1f);
        IsPaused = true;
    }

    public override void _Process(double delta)
    {
        GD.Print("Process");
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

    /// <summary>Builds a Pickup, adds it to the current scene, and registers the events for the collider</summary>
    public void DropItem(Item drop, Vector2 position)
    {
        Pickup pickup = ItemFactory.BuildPickup(drop, position);
        pickup.AddToGroup(NodeGroups.Pickups);
        _loadedScene.GetNode("%Pickups").AddChild(pickup);

        pickup.ItemPickedUp += OnItemPickedUp;
    }

    public void PlayerDropItem(Item item)
    {
        Vector2[] nsew = {
            new Vector2(0,-8), // N
			new Vector2(0,8),  // S
			new Vector2(8,0),  // E
			new Vector2(-8,0), // W
		};

        var openPositions = nsew.Where(x => _player.MoveAndCollide(x, testOnly: true) == null).ToArray();
        if (openPositions.Any())
        {
            DropItem(item, _player.Position + openPositions.Random());
            _player.Inventory.Remove(item);
        }
    }

    /// <summary>
    /// Handles the player and enemy turn timers to give some delay to player and enemy movement.
    /// </summary>
    private void HandleTimers(double delta)
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

            Level scene = ResourceLoader.Load<PackedScene>($"res://Maps/{_worldPrefix}{map}.tscn").Instantiate<Level>();
            _loadedScene = scene;
            _loadedScene.LevelLoaded += OnLevelLoaded;

            GetNode("GameContainer/GameCam").CallDeferred("add_child", scene);
        }
    }

    private void UnloadCurrentMap()
    {
        // Free existing objects we're listening to for events
        // If we don't do this, calling `GetNodesInGroup` will get these dying objects during OnLevelLoaded, since they might not have been clenaed up yet
        List<Node> entities = new List<Node>();
        entities.AddRange(GetTree().GetNodesInGroup(NodeGroups.Pickups).Cast<Node>());
        entities.AddRange(GetTree().GetNodesInGroup(NodeGroups.Stairs).Cast<Node>());
        entities.AddRange(GetTree().GetNodesInGroup(NodeGroups.Enemies).Cast<Node>());

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
        Marker2D playerSpawn = sender.GetNode<Marker2D>("PlayerSpawn");
        CurrentPlayer.Position = playerSpawn.Position;

        // Listen to events for pickups and stairs and enemies
        IEnumerable<Pickup> pickups = GetTree().GetNodesInGroup(NodeGroups.Pickups).Cast<Pickup>();
        pickups.ForEach(x => x.ItemPickedUp += OnItemPickedUp);

        IEnumerable<Stairs> stairs = GetTree().GetNodesInGroup(NodeGroups.Stairs).Cast<Stairs>();
        stairs.ForEach(x => x.StairsEntered += OnStairsEntered);

        IEnumerable<Enemy> enemies = GetTree().GetNodesInGroup(NodeGroups.Enemies).Cast<Enemy>();
        enemies.ForEach(x => x.EnemyKilled += OnEnemyKilled);
    }

    private void LoadSettings()
    {
        var settings = UserInterface.Settings.CurrentSettings;

        // Update video settings
        DisplayServer.WindowSetMode(settings.Window switch
        {
            Settings.WindowType.Maximized => DisplayServer.WindowMode.Maximized,
            Settings.WindowType.Windowed => DisplayServer.WindowMode.Windowed,
            Settings.WindowType.BorderlessWindowed => DisplayServer.WindowMode.Fullscreen,
            _ => DisplayServer.WindowMode.Windowed
        });

        Engine.MaxFps = settings.MaxFps;

        // Update Sound settings
        foreach (AudioStreamPlayer a in GetTree().GetNodesInGroup("background_noise"))
            a.VolumeDb = Mathf.LinearToDb(settings.BackgroundVolume * settings.MasterVolume);

        foreach (AudioStreamPlayer a in GetTree().GetNodesInGroup("sound_effects"))
            a.VolumeDb = Mathf.LinearToDb(settings.SoundEffectsVolume * settings.MasterVolume);

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

        Godot.Collections.Array<Node> enemies = GetTree().GetNodesInGroup(NodeGroups.Enemies);
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

    private Player LoadPlayer()
    {
        _player = GetNodeOrNull<Player>("%GameCam/Player");

        if (_player == null)
        {
            // TODO: Load settings from file

            _player = ResourceLoader.Load<PackedScene>("res://Nodes/Player.tscn").Instantiate<Player>();
            _player.Name = "Player";

            GetNode("/root/Main/GameContainer/GameCam").AddChild(_player, true);

            _player.PlayerMoved += OnPlayerMoved;
            _player.PlayerDied += OnPlayerDied;
        }

        _player = GetNode<Player>("%GameCam/Player");

        return _player;
    }

    private DungeonManager LoadDungeonManager()
    {
        DungeonManager output = GetNodeOrNull<DungeonManager>("%GameCam/DungeonMaster");

        if (output == null)
        {
            output = new DungeonManager();
            output.Name = "DungeonManager";

            GetNode("%GameCam").AddChild(output, true);
        }

        return output;
    }
}
