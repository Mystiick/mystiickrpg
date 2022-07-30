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

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        //OS.WindowMaximized = true;
        _worldPrefix = "StarterDungeon/";

        GetNode<HUD>("HUD").GetChild<Control>(0).Hide();
        GetNode<YouDied>("YouDied").GetChild<Control>(0).Hide();
        GetNode<Player>("Player").Hide();

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

    private void HandleTimers(float delta)
    {
        if (_enemyTurns.Any())
        {
            _enemyMove.Process(delta);

            if (_enemyMove.Elapsed)
            {
                _enemyMove.Reset();
                _enemyTurns.Dequeue().TakeTurn();
            }
        }
        else
        {
            _playerMove.Process(delta);

            var p = GetNode<Player>("Player");
            if (_playerMove.Elapsed && p.Health > 0)
            {
                p.CanMove = true;
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
        Player player = this.GetNode<Player>("Player");
        Position2D playerSpawn = scene.GetNode<Position2D>("PlayerSpawn");

        _loadedScene = scene;
        _loadedScene.Connect(nameof(Level.LevelLoaded), this, "OnLevelLoaded");

        this.CallDeferred("add_child", scene);
    }

    private void UnloadCurrentMap()
    {
        // Free existing objects we're listening to for events
        // If we don't do this, calling `GetNodesInGroup` will get these dying objects during OnLevelLoaded, since they might not have been clenaed up yet
        List<Node> entities = new List<Node>();
        entities.AddRange(GetTree().GetNodesInGroup("pickups").Cast<Node>());
        entities.AddRange(GetTree().GetNodesInGroup("stairs").Cast<Node>());

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
        this.MoveChild(sender, 1);

        // Move the player to the spawn point
        Player player = GetNode<Player>("Player");
        Position2D playerSpawn = sender.GetNode<Position2D>("PlayerSpawn");
        player.Position = playerSpawn.Position;

        // Listen to events for pickups and stairs
        Godot.Collections.Array pickups = GetTree().GetNodesInGroup("pickups");
        foreach (Pickup p in pickups)
        {
            if (!p.IsQueuedForDeletion())
                p.Connect(nameof(Pickup.ItemPickedUp), this, nameof(OnItemPickedUp));
        }
        Godot.Collections.Array stairs = GetTree().GetNodesInGroup("stairs");
        foreach (Stairs s in stairs)
        {
            if (!s.IsQueuedForDeletion())
                s.Connect(nameof(Stairs.StairsEntered), this, nameof(OnStairsEntered));
        }
    }

    #region | UI Events |

    private void OnMainMenuStartButtonPressed()
    {
        RestartGame();
    }

    private void OnDebugLoadLevelPressed(string level)
    {
        LoadMap(level);
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

        GetNode<Player>("Player").Reset();
        GetNode<Player>("Player").Show();

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
        ui.UpdateDeathStats(GetNode<Player>("Player"));
    }

    /// <summary>
    /// Handles the event after the player has picked up an item. 
    /// TODO: {Adds to inventory if applicable, and} updates the HUD with newest inventory
    /// </summary>
    private void OnItemPickedUp(Pickup item)
    {
        HUD hud = GetNode<HUD>("/root/Main/HUD");
        Player player = GetNode<Player>("/root/Main/Player");

        hud.UpdateHUD(player);
    }

    /// <summary>
    /// Moves the player to the level specified in the stairs' target
    /// </summary>
    private void OnStairsEntered(Stairs stairs)
    {
        LoadMap(stairs.Destination);
    }

    #endregion

}
