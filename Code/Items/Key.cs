using Godot;

public partial class Key : Item
{
    public Key() : base() { }
    public Key(Pickup source, Entity owner) : base(source, owner) { }

    public override Item Clone()
    {
        return base.CloneAs<Key>();
    }
}