using System;
using Godot;

public enum GemType
{
	Coffee,
	Vanilla,
	Milk,
	Sugar,
	Leaf,
	Black,
	Rainbow
}
static class GemTypeHelper
{

	static Random random = new Random();
	public static Color GetColor(this GemType gemType)
	{
		switch (gemType)
		{
			case GemType.Coffee:
				return new Color("#ff6e8b");
			case GemType.Sugar:
				return new Color("#72fafc");
			case GemType.Leaf:
				return new Color("#94ff9d");
			case GemType.Vanilla:
				return new Color("#ffda91");
			case GemType.Milk:
				return new Color("#fc9dec");
			case GemType.Rainbow:
				return newColor(255, 255, 255);
			case GemType.Black:
			default:
				return new Color("#727272");

		}
	}
	public static bool matchable(this GemType gemType) {
		if (gemType == GemType.Black) {
			return false;
		} else {
			return true;
		}
	}
	public static bool falls(this GemType gemType) {
		return true;
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
