using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class DeleteAllButLeadCardEffect : CardEffectIF
{

	public DeleteAllButLeadCardEffect() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		List<Tile> tiles = matchBoard.getRandomNonLeadTile();
		matchBoard.deleteGemAtPositions(tiles.Select(tile => tile.getTilePosition()).ToHashSet<Vector2>());
	}
}
