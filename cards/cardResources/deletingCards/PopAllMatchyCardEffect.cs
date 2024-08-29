using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class PopAllMatchyCardEffect : CardEffectIF
{
	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		matchBoard.checkManuallyForMatchOrDelete(getAllTilesToEffect(matchBoard, null));
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}

	public override List<Vector2> getAllTilesToEffect(MatchBoard matchBoard, Tile tile)
	{
		return matchBoard.getAllTiles().Select(tile => tile.getTilePosition()).ToList();
	}

	protected override bool bonusActive() {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard == null)
		{
			return false;
		}
		if (matchBoard.getMatchesThisTurn(GemType.Milk).Count >= 1)
			return true;
		return false;
	}

	protected override int bonusValue() {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard == null)
		{
			return 1;
		}
		return matchBoard.getMatchesThisTurn(GemType.Milk).Count;
	}
}
