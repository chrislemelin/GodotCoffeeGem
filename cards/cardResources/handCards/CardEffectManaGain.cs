using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectManaGain : CardEffectIF
{
	public CardEffectManaGain() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		mana.modifyMana(1);
	}
}
