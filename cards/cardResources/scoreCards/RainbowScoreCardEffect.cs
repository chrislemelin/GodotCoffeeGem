using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class RainbowScoreCardEffect : CardEffectIF
{

	public RainbowScoreCardEffect()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Score score = FindObjectHelper.getScore(matchBoard);
		score.addScoreFromNode(150 * getValue(), node);
	}
	public override int getValue()
	{
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard == null)
		{
			return 0;
		}
		return matchBoard.blackGemsCreatedThisTurn;
	}
	public override String getCustomText()
	{
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard == null)
		{
			return "";
		}
		HashSet<GemType> gemTypes = matchBoard.matchesThisTurn.Select(match => match.GetGemType()).ToHashSet();
		if (gemTypes.Count > 0)
		{
			return "Have matched " + string.Join(", ", gemTypes) + " (" + gemTypes.Count * 150 + ")";
		}
		return "";
	}
	public override void init()
	{
		FindObjectHelper.getMatchBoard(node).ingredientMatched += (match) => EmitSignal(SignalName.CustomTextChanged);
	}
}
