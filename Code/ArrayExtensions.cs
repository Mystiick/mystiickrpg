public static class ArrayExtensions
{
    /// <summary>
    /// Returns a random element from the given array
    /// </summary>
    public static T Random<T>(this T[] input)
    {
        return input[Godot.GD.Randi() % input.Length];
    }

    public static void ConnectAll(this Godot.Collections.Array input, string signal, Godot.Object target, string method)
    {
        foreach (Godot.Node n in input)
        {
            if (!n.IsQueuedForDeletion())
                n.Connect(signal, target, method);
        }
    }
}