using Godot;
using System;
using System.Collections.Generic;

public partial class HomeGameManager : GameManagerIF
{
	[Export] Status status;
	[Export] DayOver dayOver;
	[Export] NewCardSelection newCardSelection;

	public override void _Ready()
	{
		base._Ready();
		status.setLevel(global.currentLevel);
	}
	public override void advanceLevel() {
		GetTree().ChangeSceneToFile("res://mainScenes/game.tscn");
	}

	public void selectNewCard() {
		int cardsToChoose = 3;
		List<CardResource> cardPoolList = new List<CardResource>(cardPool.allCards);
		RandomHelper.Shuffle(cardPoolList);
		newCardSelection.setCardsToSelectFrom(cardPoolList.GetRange(0,cardsToChoose));
		newCardSelection.setCoins(0);

	}

}