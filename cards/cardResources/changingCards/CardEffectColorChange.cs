using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectColorChange : CardEffectIF
{
	[Export] int gemsToEffect;
	public CardEffectColorChange() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return gemsToEffect == 1 ? SelectionType.Single : SelectionType.Double; 
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		matchBoard.changeGemsColorAtPosition(selectedTiles, effectGemType.GetGemType());
	}
}
