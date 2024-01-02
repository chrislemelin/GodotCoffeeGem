using Godot;
using Godot.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectRapture : CardEffectIF
{
	public CardEffectRapture()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Single;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Tile tile = matchBoard.getTile(selectedTiles[0]);
		List<Vector2> positions = getAllTilesToEffect(matchBoard, tile);
		matchBoard.deleteGemAtPositions(positions);
	}

	public override List<Vector2> getAllTilesToEffect(MatchBoard matchBoard, Tile tile)
	{
		GemType gemType = tile.Gem.Type;
		List<Tile> tiles = matchBoard.getTilesWithCondition(tile => tile.Gem.Type != gemType);
		return tiles.Select(x => x.getTilePosition()).ToList();
	}
}
