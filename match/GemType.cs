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

	public static bool hasEnumValue(string value) {
		return Enum.IsDefined(typeof(GemType), value);
	}

	public static GemType fromString(string value)
	{
		return (GemType) Enum.Parse(typeof(GemType), value, true);
	}

	static Random random = new Random();
	public static Color GetColor(this GemType gemType)
	{
		switch (gemType)
		{
			case GemType.Coffee:
				return new Color("#633700");
			case GemType.Sugar:
				return new Color("#008692");
			case GemType.Leaf:
				return new Color("#2b9434");
			case GemType.Vanilla:
				return new Color("#ba951a");
			case GemType.Milk:
				return new Color("#8943a1");
			case GemType.Rainbow:
				return newColor(0, 0, 0);
			case GemType.Black:
				return new Color("#505050");
			default:
				return newColor(0, 0, 0);
		}
	}

	public static String getString(this GemType gemType)
	{
		switch (gemType)
		{
			case GemType.Coffee:
				return "coffee";
			case GemType.Sugar:
				return "sugar";
			case GemType.Leaf:
				return "tea";
			case GemType.Vanilla:
				return "vanilla";
			case GemType.Milk:
				return "milk";
			case GemType.Black:
				return "burnt";
			case GemType.Rainbow:
				return "rainbow";
			default:
				return "burnt";
		}
	}

	public static Texture2D getTexture2D(this GemType gemType) {
		switch (gemType)
		{
			case GemType.Coffee:
				return (Texture2D)GD.Load("res://placeholders/ingredients/final/Bean-pixel2.png");
			case GemType.Sugar:
				return (Texture2D)GD.Load("res://placeholders/ingredients/final/sugar-pixel2.png");
			case GemType.Leaf:
				return (Texture2D)GD.Load("res://placeholders/ingredients/final/leaf-pixel-2.png");
			case GemType.Vanilla:
				return (Texture2D)GD.Load("res://placeholders/ingredients/final/vanilla-pixel2.png");
			case GemType.Milk:
				return (Texture2D)GD.Load("res://placeholders/ingredients/final/milk-pixel-2.png");
			case GemType.Black:
				return (Texture2D)GD.Load("res://placeholders/ingredients/final/burnt-pixel-34px.png");
			default:
				return null;
		}
	}
	public static Texture2D getSmallTexture2D(this GemType gemType) {
		switch (gemType)
		{
			case GemType.Coffee:
				return (Texture2D)GD.Load("res://placeholders/ingredients/bean-pixel-100px.png");
			case GemType.Sugar:
				return (Texture2D)GD.Load("res://placeholders/ingredients/sugar-pixel-100px.png");
			case GemType.Leaf:
				return (Texture2D)GD.Load("res://placeholders/ingredients/leaf-pixel-100px.png");
			case GemType.Vanilla:
				return (Texture2D)GD.Load("res://placeholders/ingredients/vanilla-pixel-100px.png");
			case GemType.Milk:
				return (Texture2D)GD.Load("res://placeholders/ingredients/milk-pixel-100px.png");
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
