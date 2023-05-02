using Godot;
using System;

public partial class Stairs : Area2D
{
    [Signal] public delegate void StairsEnteredEventHandler(Stairs sendter);
    [Export] public Texture2D StairsUp;
    [Export] public Texture2D StairsDown;
    [Export] public StairType Type;
    [Export] public string Destination;


    public override void _Ready()
    {
        base._Ready();

        Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
        switch (Type)
        {
            case StairType.Up:
                sprite.Texture = StairsUp;
                break;
            case StairType.Down:
                sprite.Texture = StairsDown;
                break;
        }
    }

    public void OnStairsBodyEntered(PhysicsBody2D collision)
    {
        if (collision is Player p)
        {
            EmitSignal(nameof(StairsEntered), new[] { this });
        }
    }

    public enum StairType
    {
        Up,
        Down
    }
}