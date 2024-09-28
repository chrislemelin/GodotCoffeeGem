using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class LeadSinkCardEffect : CardEffectIF
{

	public LeadSinkCardEffect() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		List<Tile> tiles = matchBoard.getTilesWithColorOfGem(GemType.Lead);
		List<Tile> tilesOrdered = tiles.OrderByDescending(tile => tile.getTilePosition().Y).ToList();
		tilesOrdered.ForEach(tile => {
			matchBoard.switchGemsInPositions(tile.getTilePosition(), tile.getTilePosition() + Vector2.Down);
			//matchBoard.switchGemsInPositions(tile.getTilePosition()+ Vector2.Down, tile.getTilePosition() + Vector2.Down*2);

			});
	}
}
