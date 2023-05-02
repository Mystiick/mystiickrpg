using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
    [Signal] public delegate void StartButtonPressedEventHandler();
    [Signal] public delegate void SettingsButtonPressedEventHandler();

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
