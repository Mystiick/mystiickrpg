public class Stat
{
    public StatType Type { get; set; }
    public int Value { get; set; }

    public enum StatType
    {
        Defense,
        Attack,
        Health
    }

    public Stat Clone()
    {
        return new Stat()
        {
            Type = this.Type,
            Value = this.Value
        };
    }
}