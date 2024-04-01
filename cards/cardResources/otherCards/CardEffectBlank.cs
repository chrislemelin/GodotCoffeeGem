using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectBlank : CardEffectIF
{
	public CardEffectBlank () {

	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}

	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{

	}
}
