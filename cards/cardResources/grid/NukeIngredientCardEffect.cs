using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class NukeIngredientCardEffect : CardEffectIF
{
	public NukeIngredientCardEffect () {

	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}

	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		GemType gemType = GemTypeHelper.getRandomColor();
		while(gemType == GemType.Vanilla) {
			gemType = GemTypeHelper.getRandomColor();
		}
		matchBoard.nukeIngredientType(gemType);
	}
}
