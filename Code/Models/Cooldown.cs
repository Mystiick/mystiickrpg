public partial class Timeout
{
    public double Duration { get; set; }
    public double Remaining { get; private set; }
    public bool Elapsed { get; private set; }

    public Timeout(float duration)
    {
        Duration = duration;
        Reset();
    }

    /// <summary>Called by Main() during the normal _Process loop to update the timers</summary>
    public void Process(double delta)
    {
        if (Remaining >= 0)
            Remaining -= delta;
        else
            Elapsed = true;
    }

    public void Reset()
    {
        Elapsed = false;
        Remaining = Duration;
    }
}