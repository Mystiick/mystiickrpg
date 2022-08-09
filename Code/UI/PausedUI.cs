using Godot;
using System;

public class PausedUI : CanvasLayer
{
    [Signal] public delegate void RestartButtonPressed();
    [Signal] public delegate void SettingsPressed();

    public void UpdateDeathStats(Player p)
    {
        //TODO: Death statistics
    }

    private void OnRetryPressed()
    {
        EmitSignal(nameof(RestartButtonPressed));
    }

    private void OnSettingsPressed()
    {
        EmitSignal(nameof(SettingsPressed));
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
