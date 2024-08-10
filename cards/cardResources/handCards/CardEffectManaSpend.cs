using Godot;
using System;
using System.Collections.Generic;

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
		int manaValue = mana.manaValue;
		mana.modifyMana(-1 * manaValue);
		FindObjectHelper.getScore(matchBoard).addScoreFromNode(100 * manaValue, node);
	}
}
