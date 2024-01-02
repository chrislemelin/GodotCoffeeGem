using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class MultResetCardEffect : CardEffectIF
{

	public MultResetCardEffect()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Score score = FindObjectHelper.getScore(matchBoard);
		float oldMult = score.getMult();
		int gainValue = Math.Min((int)(oldMult - 1.0f), 3);
		hand.drawCards(gainValue);
		mana.modifyMana(gainValue);
		score.setMult(1.0f);

	}
}
