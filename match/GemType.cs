using System;
using System.Collections.Generic;
using System.Linq;
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
				return newColor(0, 0, 0);
		}
	}
	public static Texture2D getTexture2D(this GemType gemType) {
		switch (gemType)
		{
			case GemType.Coffee:
				return (Texture2D)GD.Load("res://placeholders/ingredients/bean-pixel-300px.png");
			case GemType.Sugar:
				return (Texture2D)GD.Load("res://placeholders/ingredients/sugar-pixel-300px.png");
			case GemType.Leaf:
				return (Texture2D)GD.Load("res://placeholders/ingredients/leaf-pixel-300px.png");
			case GemType.Vanilla:
				return (Texture2D)GD.Load("res://placeholders/ingredients/vanilla-pixel-300px.png");
			case GemType.Milk:
				return (Texture2D)GD.Load("res://placeholders/ingredients/milk-pixel-300px.png");
			case GemType.Black:
				return (Texture2D)GD.Load("res://placeholders/ingredients/burnt-pixel-300px.png");
			default:
				return null;
		}
	}
	// public static Texture2D getTexture2D(this GemType gemType) {
	// 	return ImageTexture.CreateFromImage(gemType.getImage());
	// }

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
	public static List<GemType> getRandomColors(int count)
	{
		List<int> ids = new List<int>{0,1,2,3,4};
		RandomHelper.Shuffle(ids);
		return ids.GetRange(0,Math.Min(count, ids.Count)).Select(id => (GemType)id).ToList();
	}

	private static Color newColor(int r, int g, int b) {
		return new Color(r/255.0f, g/255.0f, b/255.0f);
	}
}
