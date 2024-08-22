using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


[GlobalClass, Tool]
public partial class CardEffectAllConvert : CardEffectIF
{
	public CardEffectAllConvert() {
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Single;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Tile tile = matchBoard.getTile(selectedTiles[0]);
		List<Vector2> positions = getAllTilesToEffect(matchBoard, tile);
		matchBoard.changeGemsColorAtPosition(positions, effectGemType.GetGemType(), true);
	}

	public override List<Vector2> getAllTilesToEffect(MatchBoard matchBoard, Tile tile)
	{
		List<Tile> tiles = matchBoard.getTilesWithColorOfGem(tile.Gem.Type);
		return tiles.Select(x => x.getTilePosition()).ToList();
	}
}

