using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class LoseHeartSugarCardEffect : CardEffectIF
{

	public LoseHeartSugarCardEffect()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(node);
		FindObjectHelper.getScore(node).addCoins(50);
		matchBoard.changeGemsColorAtRandomPositions(GemType.Sugar, 4);
		gameManagerIF.setHealth(gameManagerIF.getHealth()-1);
	}
}
