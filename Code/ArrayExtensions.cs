using System;
using System.Collections.Generic;
using Godot;

public static class ArrayExtensions
{
	/// <summary>
	/// Returns a random element from the given array
	/// </summary>
	public static T Random<T>(this T[] input)
	{
		return input[Godot.GD.Randi() % input.Length];
	}

	public static void ConnectAll(this Godot.Collections.Array<Node> input, string signal, GodotObject target, string method)
	{
		foreach (Godot.Node n in input)
		{
			if (!n.IsQueuedForDeletion())
				n.Connect(signal, new Callable(target, method));
		}
	}

	public static void ForEach<T>(this IEnumerable<T> input, Action<T> act)
	{
		foreach (T item in input)
		{
			act(item);
		}
	}
}
