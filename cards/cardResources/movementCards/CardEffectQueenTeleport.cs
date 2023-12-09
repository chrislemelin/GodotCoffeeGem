using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectQueenTeleport : CardEffectIF
{
	public CardEffectQueenTeleport () {

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
		HashSet<Vector2> allSelectablePositions = new HashSet<Vector2>();
		HashSet<Vector2> allDirections = new HashSet<Vector2>(){Vector2.Up, Vector2.Down, Vector2.Right, Vector2.Left,
		 new Vector2(1,1), new Vector2(-1,1), new Vector2(1,-1), new Vector2(-1,-1)};
	
		return matchBoard.getTilesInDirections(tile.getPosition(), allDirections).Select(currentTile => currentTile.getPosition()).ToList();
	}


}
