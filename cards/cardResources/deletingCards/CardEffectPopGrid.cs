using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectPopGrid : CardEffectIF
{
	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Tile tile = matchBoard.getTile(selectedTiles[0]);

		matchBoard.checkManuallyForMatchOrDelete(getAllTilesToEffect(matchBoard, tile));
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Single;
	}

	public override List<Vector2> getAllTilesToEffect(MatchBoard matchBoard, Tile tile)
	{
		List<Vector2> tilesToClear = new List<Vector2>();
		tilesToClear.Add(tile.getTilePosition() + new Vector2(1, 1));
		tilesToClear.Add(tile.getTilePosition() + new Vector2(1, 0));
		tilesToClear.Add(tile.getTilePosition() + new Vector2(1, -1));

		tilesToClear.Add(tile.getTilePosition() + new Vector2(0, 1));
		tilesToClear.Add(tile.getTilePosition() + new Vector2(0, 0));
		tilesToClear.Add(tile.getTilePosition() + new Vector2(0, -1));

		tilesToClear.Add(tile.getTilePosition() + new Vector2(-1, 1));
		tilesToClear.Add(tile.getTilePosition() + new Vector2(-1, 0));
		tilesToClear.Add(tile.getTilePosition() + new Vector2(-1, -1));

		return tilesToClear;
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
}
