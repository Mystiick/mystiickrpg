using Godot;
using System;

public partial class SettingsUI : CanvasLayer
{
    [Signal] public delegate void SettingsUpdatedEventHandler();

    private static Settings _settings;
    public Settings CurrentSettings
    {
        get
        {
            if (_settings == null)
                _settings = Settings.FromFile();

            return _settings;
        }
    }

    private OptionButton _windowType;
    private SpinBox _maxFps;
    private Slider _masterSound;
    private Slider _backgroundSound;
    private Slider _soundEffects;

    public override void _Ready()
    {
        base._Ready();

        // Save off a reference of all the controls
        _windowType = GetNode<OptionButton>("%WindowType");
        _maxFps = GetNode<SpinBox>("%MaxFps");
        _masterSound = GetNode<Slider>("%MasterSound");
        _backgroundSound = GetNode<Slider>("%BackgroundSound");
        _soundEffects = GetNode<Slider>("%SoundEffects");

        // Populate controls with the values from the file
        PopulateForm();
    }

    /// <summary>
    /// Saves & closes the form, and signals that the settings have been updated.
    /// </summary>
    public void OnSavePressed()
    {
        SaveSettings();
        EmitSignal(nameof(SettingsUpdated));

        Hide();
    }

    /// <summary>
    /// Saves the form, and signals that the settings have been updated.
    /// Leaves the form open.
    /// </summary>
    public void OnApplyPressed()
    {
        SaveSettings();
        EmitSignal(nameof(SettingsUpdated));
    }

    /// <summary>
    /// Resets the form, and hides the screen
    /// </summary>
    public void OnCancelPressed()
    {
        PopulateForm();
        Hide();
    }

    /// <summary>
    /// Populates the Settings screen with all the values from the loaded file
    /// </summary>
    private void PopulateForm()
    {
        // Rendering window setting
        _windowType.Clear();
        _windowType.AddItem("Windowed");
        _windowType.AddItem("Maximized");
        _windowType.AddItem("Borderless Window");
        _windowType.Select((int)CurrentSettings.Window);

        _maxFps.Value = CurrentSettings.MaxFps;

        // Audio sliders
        _masterSound.Value = CurrentSettings.MasterVolume;
        _backgroundSound.Value = CurrentSettings.BackgroundVolume;
        _soundEffects.Value = CurrentSettings.SoundEffectsVolume;
    }

    /// <summary>
    /// Populates CurrentSettings with all of the current values from the form, and saves them to the file
    /// </summary>
    private void SaveSettings()
    {
        _settings = new Settings()
        {
            Window = (Settings.WindowType)_windowType.Selected,
            MaxFps = (int)_maxFps.Value,
            MasterVolume = (float)_masterSound.Value,
            BackgroundVolume = (float)_backgroundSound.Value,
            SoundEffectsVolume = (float)_soundEffects.Value
        };

        _settings.ToFile();
    }
}
