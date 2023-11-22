using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class SwitchCardEffect : CardEffectIF
{

	public SwitchCardEffect() {
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Double;
	}


	public override void doEffect(MatchBoard matchBoard, Mana mana, List<Vector2> selectedTiles)
	{
		Vector2 selectedTile1 = selectedTiles[0];
		Vector2 selectedTile2 = selectedTiles[1];
		matchBoard.switchGemsInPositions(selectedTile1, selectedTile2);
	}
}
