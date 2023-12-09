using Godot;
using System;

public static class RangeHelper
{
	public static string getString(float range) {
		if (range == 1.0f) {
			return "orthogonal adjacent";
		}
		if (range < 2.0f && range > Math.Sqrt(2)) {
			return "diagonally adjacent";
		}
		return range.ToString() + " tiles away";;
	}

	//public bool diagonallyAdjacent

}
