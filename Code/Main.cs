using Godot;
using System.Linq;

public class Main : Node
{
    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        //OS.WindowMaximized = true;
        LoadMap("StarterDungeon");
    }

    /// <summary>
    /// Loads the specified map by name, moves the player to the spawn location, and moves the map to the top of the node tree (right under the background).
    /// </summary>
    private void LoadMap(string map)
    {
        Node scene = ResourceLoader.Load<PackedScene>($"res://Maps/{map}.tscn").Instance();
        Player player = this.GetNode<Player>("Player");
        Position2D playerSpawn = scene.GetNode<Position2D>("PlayerSpawn");

        this.AddChild(scene);
        this.MoveChild(scene, 1);

        player.Position = playerSpawn.Position;

        Godot.Collections.Array pickups = GetTree().GetNodesInGroup("pickups");
        foreach (Pickup p in pickups)
        {
            p.Connect(nameof(Pickup.ItemPickedUp), this, nameof(OnItemPickedUp));
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
        if (item is Key)
        {
            GetNode<HUD>("HUD").UpdateKeys(GetNode<Player>("Player").Keys);
        }
        else if (item is Chicken)
        {

        }
    }
}
