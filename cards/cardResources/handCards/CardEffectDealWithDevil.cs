using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectDealWithDevil : CardEffectIF
{
	public CardEffectDealWithDevil()
	{

	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		FindObjectHelper.getScore(matchBoard).modifyTurnsRemaining(-1);
		hand.drawCards(3);
		mana.modifyMana(2);
	}
}
