using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class MultIncreaseCardEffect : CardEffectIF
{
	[Export] private float multIncrease;

	public MultIncreaseCardEffect() {
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}

	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		FindObjectHelper.getScore(matchBoard).addMult(multIncrease);
		hand.drawCards(1);
	}
}
