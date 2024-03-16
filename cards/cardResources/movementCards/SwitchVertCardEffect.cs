using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class SwitchVertCardEffect : CardEffectIF
{
	public SwitchVertCardEffect()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Double;
	}

	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Vector2 selectedTile1 = selectedTiles[0];
		Vector2 selectedTile2 = selectedTiles[1];
		matchBoard.switchGemsInPositions(selectedTile1, selectedTile2);
	}

	public override List<Vector2> getAllTilesSelectableAfterFirstSelection(MatchBoard matchBoard, Tile tile)
	{
		HashSet<Vector2> allDirections = new HashSet<Vector2>() { Vector2.Up, Vector2.Down };
		return matchBoard.getTilesInDirections(tile.getTilePosition(), allDirections, (int)getRange()).Select(currentTile => currentTile.getTilePosition()).ToList();
	}
}
