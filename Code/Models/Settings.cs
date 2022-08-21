using Godot;
using Newtonsoft.Json;

public class Settings
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
        var f = new File();

        // Create a default file if one doesn't exist yet
        if (!f.FileExists(fileName))
        {
            new Settings()
            {
                Window = Settings.WindowType.Windowed,
                MasterVolume = 1,
                BackgroundVolume = 1,
                SoundEffectsVolume = 1
            }.ToFile();
        }

        f.Open(fileName, File.ModeFlags.Read);
        return JsonConvert.DeserializeObject<Settings>(f.GetLine());
    }


    /// <summary>
    //// Saves the settings to the specified file
    /// </summary>
    public void ToFile(string fileName = "user://settings.json")
    {
        var f = new File();

        f.Open(fileName, File.ModeFlags.WriteRead);
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