using Godot;
using System;

public class HUD : CanvasLayer
{
    public void UpdateHealth(int current, int max)
    {
        GetNode<Label>("Health").Text = $"Health: {current}/{max}";
    }
}
