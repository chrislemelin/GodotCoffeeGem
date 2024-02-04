using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectCardBurst : CardEffectIF
{
	public CardEffectCardBurst() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		int valueMod = 0;
		if (matchBoard.getTilesWithAddon(GemAddonType.Card).Count > 2) {
			valueMod = 1;
		}
		hand.drawCards(getValue() + valueMod);
	}
}
