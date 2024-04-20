using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class PopColCardEffect : CardEffectIF
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
		List<Tile> tilesToClear = new List<Tile>();
		tilesToClear.Add(tile);
		tilesToClear.AddRange(matchBoard.getTilesInDirection(tile.getTilePosition(), Vector2.Up));
		tilesToClear.AddRange(matchBoard.getTilesInDirection(tile.getTilePosition(), Vector2.Down));
		return tilesToClear.Select(x => x.getTilePosition()).ToList();
	}
}
