using System;
using Godot;

public enum GemType
{
	Blue,
	Red,
	Green,
	Yellow,
	Purple
}
static class GemTypeHelper
{
	static Random random = new Random();
	public static Color GetColor(this GemType gemType)
	{
		switch (gemType)
		{
			case GemType.Red:
				return new Color(.8f, .3f, .3f);
			case GemType.Blue:
				return new Color(.3f, .3f, .8f);
			case GemType.Green:
				return new Color(.3f, .8f, .3f);
			case GemType.Yellow:
				return new Color(.8f, .8f, .3f);
			case GemType.Purple:
				return new Color(.8f, .3f, .8f);
			default:
				return new Color(0, 0, 0);

		}
	}
	public static GemType getRandomColor()
	{
		return (GemType)random.Next(3);
	}
}
