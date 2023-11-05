using Godot;
using System;
using System.Collections.Generic;

public static class RandomHelper
{

	public static void Shuffle<T>(List<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int n1 = n + 1;
			int k = GD.RandRange(0, n);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
