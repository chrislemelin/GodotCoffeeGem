using Godot;
using System;

public enum CardEffectGemType
{
	None,
	Milk,
	Vanilla,
	Leaf,
	Coffee,
	Sugar,
	Black,
	Rainbow  
}
static class CardEffectGemTypeHelper
{
	public static GemType GetGemType(this CardEffectGemType cardEffectGemType)
	{
		switch (cardEffectGemType)
		{
			case CardEffectGemType.Milk:
				return GemType.Milk;
			case CardEffectGemType.Vanilla:
				return GemType.Vanilla;
			case CardEffectGemType.Leaf:
				return GemType.Leaf;
			case CardEffectGemType.Coffee:
				return GemType.Coffee;
			case CardEffectGemType.Sugar:
				return GemType.Sugar;
			case CardEffectGemType.Black:
			default:
				return GemType.Black;
		}
	}
}

