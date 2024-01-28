using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class EnhanceCardEffect : CardEffectIF
{

	public EnhanceCardEffect()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Single;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Score score = FindObjectHelper.getScore(matchBoard);
		ColorUpgrade colorUpgrade = new ColorUpgrade();
		colorUpgrade.gemType = matchBoard.getTile(selectedTiles[0]).Gem.Type;
		colorUpgrade.baseIncrease = 50;
		colorUpgrade.temporary = true;
		score.addColorUpgrade(colorUpgrade);
	}
}
