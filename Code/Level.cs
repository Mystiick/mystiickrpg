using Godot;
using System;

public partial class Level : TileMap
{
	[Signal] public delegate void LevelLoadedEventHandler(Level me);
	public override void _Ready()
	{
		base._Ready();
		EmitSignal(nameof(LevelLoaded), new[] { this });
	}
}
