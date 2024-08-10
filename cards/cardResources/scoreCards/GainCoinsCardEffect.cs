using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class GainCoinsCardEffect : CardEffectIF
{
	[Export] private int bonusCoins = 0;
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
		if (bonusActive()) {
			score.addCoins(bonusCoins);
		}
	}
	
	protected override bool bonusActive() {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard == null)
		{
			return false;
		}
		if (matchBoard.getMatchesThisTurn(GemType.Sugar).Count >= 1)
			return true;
		return false;
	}
	public override void init()
	{
		FindObjectHelper.getMatchBoard(node).ingredientMatched += (match) => EmitSignal(SignalName.CustomTextChanged);
		FindObjectHelper.getNewTurnButton(node).TurnCleanUp += () => EmitSignal(SignalName.CustomTextChanged);
	}
}
