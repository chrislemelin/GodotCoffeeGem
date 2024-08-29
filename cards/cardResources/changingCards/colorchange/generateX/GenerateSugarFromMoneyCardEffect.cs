using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class GenerateSugarFromMoneyCardEffect : CardEffectIF
{
	[Export] public int coinValue = 25;

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		int totalValue = FindObjectHelper.getGameManager(node).getCoins() / coinValue;
		matchBoard.changeGemsColorAtRandomPositions(GemType.Sugar, totalValue);
	}
}
