using Godot;
using System;

public partial class Torch : Area2D
{
    PointLight2D _light;

    public override void _Ready()
    {
        base._Ready();
        _light = GetNode<PointLight2D>("PointLight2D");
        _light.Enabled = false;
    }

    public void OnBodyEntered(PhysicsBody2D collision)
    {
        if (collision is Player)
        {
            _light.Enabled = true;
        }
    }
}
