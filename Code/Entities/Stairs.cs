using Godot;
using System;

public class Stairs : Area2D
{
    [Signal] public delegate void StairsEntered(Stairs sendter);
    [Export] public Texture StairsUp;
    [Export] public Texture StairsDown;
    [Export] public StairType Type;
    [Export] public string Destination;


    public override void _Ready()
    {
        base._Ready();

        Sprite sprite = GetNode<Sprite>("Sprite");
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