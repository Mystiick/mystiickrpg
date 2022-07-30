using Godot;
using System;

public class MainMenu : CanvasLayer
{
    [Signal] public delegate void StartButtonPressed();

    public void OnStartPressed()
    {
        EmitSignal(nameof(StartButtonPressed));
    }
    public void OnExitPressed()
    {
        GetTree().Quit();
    }
}
