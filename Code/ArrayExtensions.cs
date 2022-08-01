public static class ArrayExtensions
{
    public static T Random<T>(this T[] input)
    {
        return input[Godot.GD.Randi() % input.Length];
    }
}