using Godot;
using System;

public class Torch : Area2D
{
    Light2D _light;

    public override void _Ready()
    {
        _light = GetNode<Light2D>("Light2D");
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
