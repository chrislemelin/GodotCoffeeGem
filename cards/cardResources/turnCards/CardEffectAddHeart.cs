using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectAddHeart : CardEffectIF
{

	public CardEffectAddHeart()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		GameManagerIF gameManager = FindObjectHelper.getGameManager(matchBoard);
		gameManager.setHealth(gameManager.getHealth() + 1);
		matchBoard.removeColumn();
	}
}
