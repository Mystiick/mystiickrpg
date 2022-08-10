using Godot;
using System.Linq;

public class UIManager : Control
{
    public HUD HUD { get; private set; }

    public DebugUI Debug { get; private set; }
    [Signal] public delegate void DebugLoadLevel(string level);

    public MainMenu MainMenu { get; private set; }
    [Signal] public delegate void MainMenuStartButtonPressed();


    public PausedUI Paused { get; private set; }
    [Signal] public delegate void PausedRestartButtonPressed();

    public SettingsUI Settings { get; private set; }
    [Signal] public delegate void SettingsUpdated();

    public override void _Ready()
    {
        HUD = GetNode<HUD>("HUD");
        Debug = GetNode<DebugUI>("DebugUI");
        MainMenu = GetNode<MainMenu>("MainMenu");
        Paused = GetNode<PausedUI>("Paused");
        Settings = GetNode<SettingsUI>("SettingsUI");
    }

    public void HideAll()
    {
        HUD.Hide();
        // Debug.Hide();
        MainMenu.Hide();
        Paused.Hide();
        Settings.Hide();
    }

    // Main Menu events
    public void OnMainMenuSettingsButtonPressed()
    {
        Settings.Show();
    }
    public void OnMainMenuStartButtonPressed()
    {
        HideAll();
        HUD.CallDeferred("show");

        EmitSignal(nameof(MainMenuStartButtonPressed));
    }

    // Settings events
    public void OnSettingsUpdated()
    {
        EmitSignal(nameof(SettingsUpdated));
    }

    // Paused events
    public void OnPausedRestartButtonPressed()
    {
        Paused.Hide();
        EmitSignal(nameof(PausedRestartButtonPressed));
    }
    public void OnPausedSettingsButtonPressed()
    {
        Settings.Show();
    }

    // DebugUI
    public void OnDebugUILoadLevelPressed(string level)
    {
        EmitSignal(nameof(DebugLoadLevel), level);
    }
}
