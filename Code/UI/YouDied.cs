using Godot;
using System;

public class YouDied : CanvasLayer
{
    [Signal] public delegate void RestartButtonPressed();
    [Signal] public delegate void MainMenuButtonPressed();

    public void UpdateDeathStats(Player p)
    {
        //TODO: Death statistics
    }

    private void OnRetryPressed()
    {
        EmitSignal(nameof(RestartButtonPressed));
    }

    private void OnMainMenuPressed()
    {
        EmitSignal(nameof(MainMenuButtonPressed));
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
