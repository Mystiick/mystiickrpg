using Godot;
using System.Linq;

public class Main : Node
{
    private Node _loadedScene;
    private string _worldPrefix;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        //OS.WindowMaximized = true;
        _worldPrefix = "StarterDungeon/";
        LoadMap("Level1");
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("debug"))
        {
            GetNode<DebugUI>("DebugUI").ShowLevelSelect();
        }
    }

    /// <summary>
    /// Loads the specified map by name, moves the player to the spawn location, and moves the map to the top of the node tree (right under the background).
    /// </summary>
    private void LoadMap(string map)
    {
        if (_loadedScene != null)
            _loadedScene.QueueFree();

        Node scene = ResourceLoader.Load<PackedScene>($"res://Maps/{_worldPrefix}{map}.tscn").Instance();
        Player player = this.GetNode<Player>("Player");
        Position2D playerSpawn = scene.GetNode<Position2D>("PlayerSpawn");

        _loadedScene = scene;
        _loadedScene.Connect(nameof(Level.OnLevelLoaded), this, "OnLevelLoaded");

        this.CallDeferred("add_child", scene);
    }

    private void OnLevelLoaded(Level sender)
    {
        this.MoveChild(sender, 1);

        Player player = GetNode<Player>("Player");
        Position2D playerSpawn = sender.GetNode<Position2D>("PlayerSpawn");
        player.Position = playerSpawn.Position;

        Godot.Collections.Array pickups = GetTree().GetNodesInGroup("pickups");
        foreach (Pickup p in pickups)
        {
            p.Connect(nameof(Pickup.ItemPickedUp), this, nameof(OnItemPickedUp));
        }
        Godot.Collections.Array stairs = GetTree().GetNodesInGroup("stairs");
        foreach (Stairs s in stairs)
        {
            s.Connect(nameof(Stairs.StairsEntered), this, nameof(OnStairsEntered));
        }
    }

    /// <summary>
    /// Handles the event after the player has moved. Updates enemies and allows them to move/attack.
    /// </summary>
    private void OnPlayerMoved()
    {
        Godot.Collections.Array enemies = GetTree().GetNodesInGroup("enemies");
        foreach (Enemy e in enemies)
        {
            e.TakeTurn();
        }
    }

    /// <summary>
    /// Handles the event after the player has picked up an item. 
    /// TODO: {Adds to inventory if applicable, and} updates the HUD with newest inventory
    /// </summary>
    private void OnItemPickedUp(Pickup item)
    {
        HUD hud = GetNode<HUD>("/root/Main/HUD");
        Player player = GetNode<Player>("/root/Main/Player");

        if (item is Key)
        {
            hud.UpdateKeys(player.Keys);
        }
        else if (item is Chicken)
        {

        }
    }

    private void OnDebugLoadLevelPressed(string level)
    {
        LoadMap(level);
    }


    private void OnStairsEntered(Stairs stairs)
    {
        LoadMap(stairs.Destination);
    }
}
