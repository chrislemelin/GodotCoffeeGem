using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class LeadRiseCardEffect : CardEffectIF
{

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		List<Tile> tiles = matchBoard.getTilesWithColorOfGem(GemType.Lead);
		List<Tile> tilesOrdered = tiles.OrderBy(tile => tile.getTilePosition().Y).ToList();
		tilesOrdered.ForEach(tile => {
			tile.Gem.incrementLeadLevel(2);
			matchBoard.switchGemsInPositions(tile.getTilePosition(), tile.getTilePosition() + Vector2.Up);
			});

	}
}
