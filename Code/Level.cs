using Godot;
using System;

public class Level : TileMap
{
    [Signal] public delegate void LevelLoaded(Level me);
    public override void _Ready()
    {
        base._Ready();
        EmitSignal(nameof(LevelLoaded), new[] { this });
    }
}
