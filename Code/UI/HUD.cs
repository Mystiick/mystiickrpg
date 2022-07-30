using Godot;
using System;

public class HUD : CanvasLayer
{
    public void UpdateHUD(Player p)
    {
        UpdateHealth(p.Health, p.MaxHealth);
        UpdateKeys(p.Keys);
    }

    public void UpdateHealth(int current, int max)
    {
        GetNode<Label>("Base/Health").Text = $"Health: {current}/{max}";
    }

    public void UpdateKeys(int current)
    {
        GetNode<Label>("Base/Keys").Text = $"Keys: {current}";
    }
}
