using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectAddManaCard : CardEffectIF
{

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		matchBoard.addGemAddons(matchBoard.getRandomNonSpecialNonAddonTiles(3 ).Select(x => x.getTilePosition()).ToList(), GemAddonType.Card);
		matchBoard.addGemAddons(matchBoard.getRandomNonSpecialNonAddonTiles(4).Select(x => x.getTilePosition()).ToList(), GemAddonType.Mana);
	}

}
