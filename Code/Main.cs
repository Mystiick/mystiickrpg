using Godot;
using System;

public class Main : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //OS.WindowMaximized = true;
        LoadMap("StarterDungeon");
    }

    internal void LoadMap(string map)
    {
        Node scene = ResourceLoader.Load<PackedScene>($"res://Maps/{map}.tscn").Instance();
        Player player = this.GetNode<Player>("Player");
        Position2D playerSpawn = scene.GetNode<Position2D>("PlayerSpawn");

        this.AddChild(scene);
        this.MoveChild(scene, 1);

        player.Position = playerSpawn.Position;
    }

    public void OnPlayerMoved()
    {
        GetTree().CallGroup("enemies", "TakeTurn");
    }
}
