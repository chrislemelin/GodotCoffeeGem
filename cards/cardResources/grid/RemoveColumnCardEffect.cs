using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class RemoveColumnCardEffect : CardEffectIF
{
	public RemoveColumnCardEffect () {

	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}

	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		matchBoard.removeColumn();
	}
}
