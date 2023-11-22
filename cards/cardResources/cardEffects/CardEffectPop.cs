using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]

public partial class CardEffectPop : CardEffectIF
{
	public CardEffectPop() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Single;
	}


	public override void doEffect(MatchBoard matchBoard, Mana mana, List<Vector2> selectedTiles)
	{
		Vector2 selectedTile = selectedTiles[0];
		matchBoard.deleteGemAtPosition(selectedTile);
	}


}
