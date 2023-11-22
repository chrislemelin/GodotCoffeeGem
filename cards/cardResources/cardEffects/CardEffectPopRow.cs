using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectPopRow : CardEffectIF
{
	public override void doEffect(MatchBoard matchBoard, Mana mana, List<Vector2> selectedTiles)
	{
		Tile tile = matchBoard.getTile(selectedTiles[0]);

		matchBoard.deleteGemAtPositions(getAllTilesToEffect(matchBoard, tile));
	}

	public override List<Vector2> getAllTilesToEffect(MatchBoard matchBoard, Tile tile) {
		List<Tile> tilesToClear = new List<Tile>();
		tilesToClear.Add(tile);
		tilesToClear.AddRange(matchBoard.getTilesInDirection(tile.getPosition(),Vector2.Right));
		tilesToClear.AddRange(matchBoard.getTilesInDirection(tile.getPosition(),Vector2.Left));
		return tilesToClear.Select(x => x.getPosition()).ToList();
	}
}
