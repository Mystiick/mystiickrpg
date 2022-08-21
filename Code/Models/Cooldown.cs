public class Timeout
{
    public float Duration { get; set; }
    public float Remaining { get; private set; }
    public bool Elapsed { get; private set; }

    public Timeout(float duration)
    {
        Duration = duration;
        Reset();
    }

    /// <summary>Called by Main() during the normal _Process loop to update the timers</summary>
    public void Process(float delta)
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