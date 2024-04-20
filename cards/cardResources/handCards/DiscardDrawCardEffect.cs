using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class DiscardDrawCardEffect : CardEffectIF
{
	public DiscardDrawCardEffect() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		int handValue = hand.getAllCards().Count - 1;
		hand.discardHand();
		hand.drawCards(handValue);
	}
}
