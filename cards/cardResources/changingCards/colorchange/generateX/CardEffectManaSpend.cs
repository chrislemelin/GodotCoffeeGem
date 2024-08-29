using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectManaSpend : CardEffectIF
{
	public CardEffectManaSpend() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		int totalValue = getValue() * spendXValue;
		matchBoard.changeGemsColorAtRandomPositions(effectGemType.GetGemType(), totalValue);
	}
}
