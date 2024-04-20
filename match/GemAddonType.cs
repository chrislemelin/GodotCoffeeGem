using Godot;
using System;

public enum GemAddonType
{
	None,
	Mana,
	Card,
	Combo,
	Mult,
	Hidden,
	Lock,
	Money,
	MetaCoin
}

static class GemAddonTypeHelper
{
	static Random random = new Random();

	//gets Random addon (ignoring none)
	public static GemAddonType getRandomAddon()
	{
		return (GemAddonType)(random.Next(Enum.GetNames(typeof(GemAddonType)).Length - 1) + 1);
	}
}
