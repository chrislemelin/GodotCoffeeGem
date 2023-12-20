using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class SwitchVertCardEffect : CardEffectIF
{

	public SwitchVertCardEffect() {
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Double;
	}


	public override List<Vector2> getAllTilesSelectableAfterFirstSelection(MatchBoard matchBoard, Tile tile){
		return new List<Vector2>(){tile.getTilePosition() + new Vector2(0,-1), tile.getTilePosition() + new Vector2(0,1)};
	}

	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Vector2 selectedTile1 = selectedTiles[0];
		Vector2 selectedTile2 = selectedTiles[1];
		matchBoard.switchGemsInPositions(selectedTile1, selectedTile2);
	}
}
