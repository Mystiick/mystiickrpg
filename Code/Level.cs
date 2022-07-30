using Godot;
using System;

public class Level : TileMap
{
    [Signal] public delegate void OnLevelLoaded(Level me);
    public override void _Ready()
    {
        EmitSignal(nameof(OnLevelLoaded), new[] { this });
    }
}
