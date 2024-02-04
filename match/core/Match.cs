using Godot;
using System;
using System.Collections.Generic;

public partial class Match : Resource
{
	public List<Gem> ingredients;
	public GemType GetGemType() {
		foreach (Gem gem in ingredients)
		{
			if (gem.Type != GemType.Rainbow)
			{
				return gem.Type;
			}
		}
		return GemType.Rainbow;
	}
}
