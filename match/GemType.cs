using System;
using Godot;

public enum GemType
{
	Blue,
	Red,
	Green,
	Yellow,
	Purple,
	Black
}
static class GemTypeHelper
{

	static Random random = new Random();
	public static Color GetColor(this GemType gemType)
	{
		switch (gemType)
		{
			case GemType.Red:
				return newColor(196, 26, 0);
			case GemType.Blue:
				return newColor(0, 5, 145);
			case GemType.Green:
				return newColor(4, 184, 25);
			case GemType.Yellow:
				return newColor(209, 154, 0);
			case GemType.Purple:
				return newColor(132, 15, 145);
			case GemType.Black:
			default:
				return new Color(.25f, .25f, .25f);

		}
	}
	public static bool matchable(this GemType gemType) {
		if (gemType == GemType.Black) {
			return false;
		} else {
			return true;
		}
	}
	public static bool getsPointsFromMatching(this GemType gemType) {
		if (gemType == GemType.Black) {
			return false;
		} else {
			return true;
		}
	}
	//gets random gem (excluding black)
	public static GemType getRandomColor()
	{
		return (GemType)random.Next(5);
	}
	private static Color newColor(int r, int g, int b) {
		return new Color(r/255.0f, g/255.0f, b/255.0f);
	}
}
