using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectCardSpend : CardEffectIF
{
	public CardEffectCardSpend() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		int handValue = hand.getAllCards().Count;
		hand.discardHand();
		FindObjectHelper.getScore(matchBoard).addScore(75 * handValue);
	}
}
