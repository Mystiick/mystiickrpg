using Godot;
using Newtonsoft.Json;

public partial class Settings
{
    public WindowType Window { get; set; }
    public int MaxFps { get; set; }

    public float MasterVolume { get; set; }
    public float BackgroundVolume { get; set; }
    public float SoundEffectsVolume { get; set; }

    public Settings() { }

    /// <summary>
    /// Reads (or creates if one does not exist) the specified settings file
    /// </summary>
    public static Settings FromFile(string fileName = "user://settings.json")
    {
        // Create a default file if one doesn't exist yet
        if (!FileAccess.FileExists(fileName))
        {
            new Settings()
            {
                Window = Settings.WindowType.Windowed,
                MasterVolume = 1,
                BackgroundVolume = 1,
                SoundEffectsVolume = 1
            }.ToFile();
        }

        var f = FileAccess.Open(fileName, FileAccess.ModeFlags.Read);
        return JsonConvert.DeserializeObject<Settings>(f.GetLine());
    }


    /// <summary>
    //// Saves the settings to the specified file
    /// </summary>
    public void ToFile(string fileName = "user://settings.json")
    {
        using var f = FileAccess.Open(fileName, FileAccess.ModeFlags.WriteRead);
        f.StoreLine(JsonConvert.SerializeObject(this));
        f.Close();
    }

    public enum WindowType
    {
        Windowed,
        Maximized,
        BorderlessWindowed
    }
}