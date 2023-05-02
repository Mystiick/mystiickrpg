using Godot;
using System;

public partial class PausedUI : CanvasLayer
{
    [Signal] public delegate void RestartButtonPressedEventHandler();
    [Signal] public delegate void SettingsPressedEventHandler();

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
