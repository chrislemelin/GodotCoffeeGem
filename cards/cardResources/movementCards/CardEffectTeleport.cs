using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
		List<Tile> allSelectablePositions = new List<Tile>();
		allSelectablePositions.AddRange(matchBoard.getTilesInDirection(tile.getTilePosition(), Vector2.Right));
		allSelectablePositions.AddRange(matchBoard.getTilesInDirection(tile.getTilePosition(), Vector2.Left));
		allSelectablePositions.AddRange(matchBoard.getTilesInDirection(tile.getTilePosition(), Vector2.Up));
		allSelectablePositions.AddRange(matchBoard.getTilesInDirection(tile.getTilePosition(), Vector2.Down));

		return allSelectablePositions.Select(x => x.getTilePosition()).ToList();
	}


}
