using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class AddTwoAddonTargetCardEffect : CardEffectIF
{
	[Export] public GemAddonType gemAddonType;
	public AddTwoAddonTargetCardEffect()
	{

	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Double;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		 matchBoard.addGemAddons(selectedTiles, gemAddonType);
	}

}

