using Godot;
using System;

public class HUD : CanvasLayer
{
    public override void _Ready()
    {
    }

    public void UpdateHealth(int current, int max)
    {
        GetNode<Label>("Health").Text = $"Health: {current}/{max}";
    }

    public void UpdateKeys(int current)
    {
        GetNode<Label>("Keys").Text = $"Keys: {current}";
    }
}
