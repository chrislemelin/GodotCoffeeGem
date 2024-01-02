using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectAddTurn : CardEffectIF
{

	public CardEffectAddTurn()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Score score = FindObjectHelper.getScore(matchBoard);
		score.setTurnsRemaining(score.getTurnsRemaining() + getValue());
	}
}
