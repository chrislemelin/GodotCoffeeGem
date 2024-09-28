using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class LeadGenerateCardEffect : CardEffectIF
{

	public LeadGenerateCardEffect() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		matchBoard.generateRandomLead(getValue());
	}
}
