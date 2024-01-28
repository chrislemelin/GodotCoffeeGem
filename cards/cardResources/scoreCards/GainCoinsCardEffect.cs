using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class GainCoinsCardEffect : CardEffectIF
{

	public GainCoinsCardEffect()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Score score = FindObjectHelper.getScore(matchBoard);
		score.addCoins(getValue());
	}
}
