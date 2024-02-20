using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectColorChangeX : CardEffectIF
{
	public CardEffectColorChangeX() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Single; 
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Tile tile = matchBoard.getTile(selectedTiles[0]);
		matchBoard.changeGemsColorAtPosition(getAllTilesToEffect(matchBoard, tile), effectGemType.GetGemType());
	}


	public override List<Vector2> getAllTilesToEffect(MatchBoard matchBoard, Tile tile){
		return new List<Vector2>(){tile.getTilePosition(), 
		tile.getTilePosition() + new Vector2(1,1), 
		tile.getTilePosition() + new Vector2(-1,1), 
		tile.getTilePosition() + new Vector2(1,-1), 
		tile.getTilePosition() + new Vector2(-1,-1)};
	}
}
