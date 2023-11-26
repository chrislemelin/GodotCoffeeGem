using Godot;
using System;
using System;
using Godot;

public enum CardEffectGemType
{
	None,
	Blue,
	Red,
	Green,
	Yellow,
	Purple,
	Black,
	Rainbow  
}
static class CardEffectGemTypeHelper
{
	public static GemType GetGemType(this CardEffectGemType cardEffectGemType)
	{
		switch (cardEffectGemType)
		{
			case CardEffectGemType.Red:
				return GemType.Red;
			case CardEffectGemType.Blue:
				return GemType.Blue;
			case CardEffectGemType.Green:
				return GemType.Green;
			case CardEffectGemType.Yellow:
				return GemType.Yellow;
			case CardEffectGemType.Purple:
				return GemType.Purple;
			case CardEffectGemType.Black:
			default:
				return GemType.Black;

		}
	}
}

