using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectTeleport : CardEffectIF
{
	public CardEffectTeleport () {

	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Double;
	}

	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Vector2 selectedTile1 = selectedTiles[0];
		Vector2 selectedTile2 = selectedTiles[1];
		matchBoard.switchGemsInPositions(selectedTile1, selectedTile2, true);
	}


	public override List<Vector2> getAllTilesSelectableAfterFirstSelection(MatchBoard matchBoard, Tile tile){
		List<Vector2> allSelectablePositions = new List<Vector2>();
		int sizeX = matchBoard.sizeX;
		int sizeY = matchBoard.sizeY;
		for (int x = 0; x < sizeX; x++) {
			allSelectablePositions.Add(new Vector2(x, tile.y));
		}
		  for (int y = 0; y < sizeY; y++) {
			allSelectablePositions.Add(new Vector2(tile.x, y));
		}
		return allSelectablePositions;
	}


}
