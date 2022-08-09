using Godot;
using System;

public class MainMenu : CanvasLayer
{
    [Signal] public delegate void StartButtonPressed();
    [Signal] public delegate void SettingsButtonPressed();

    public void OnStartPressed()
    {
        EmitSignal(nameof(StartButtonPressed));
    }

    public void OnSettingsPressed()
    {
        EmitSignal(nameof(SettingsButtonPressed));
    }

    public void OnExitPressed()
    {
        GetTree().Quit();
    }

}
