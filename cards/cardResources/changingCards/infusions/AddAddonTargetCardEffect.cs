using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class AddAddonTargetCardEffect : CardEffectIF
{
	[Export] public GemAddonType gemAddonType;
	public AddAddonTargetCardEffect()
	{

	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Single;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		 matchBoard.addGemAddons(selectedTiles, gemAddonType);
	}

}

