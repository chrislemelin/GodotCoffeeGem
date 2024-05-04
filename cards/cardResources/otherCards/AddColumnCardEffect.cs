using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class AddColumnCardEffect : CardEffectIF
{
	public AddColumnCardEffect () {

	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}

	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		matchBoard.addColumn();
	}
}
