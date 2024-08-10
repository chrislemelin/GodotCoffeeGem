using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class MatchScoreCardEffect : CardEffectIF
{

	public MatchScoreCardEffect()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Score score = FindObjectHelper.getScore(matchBoard);
		score.addScoreFromNode(75 * getValue(), node);
	}
	public override int getValue()
	{
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard == null)
		{
			return 0;
		}
		return matchBoard.matchesThisTurn.Where(match => match.GetGemType() == effectGemType.GetGemType()).Count();
	}

	public override string getValueString()
	{
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard == null)
		{
			return "";
		}
		return getValue() * 75 + "";
	}
	public override void init()
	{
		FindObjectHelper.getMatchBoard(node).ingredientMatched += (match) => EmitSignal(SignalName.ValueChanged);
	}
}
