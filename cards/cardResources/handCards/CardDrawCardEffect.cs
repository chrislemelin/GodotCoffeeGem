using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardDrawCardEffect : CardEffectIF
{
	public CardDrawCardEffect() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		hand.drawCards(getValue());
	}
}
