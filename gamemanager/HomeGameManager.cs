using Godot;
using System;
using System.Collections.Generic;

public partial class HomeGameManager : GameManagerIF
{
	[Export] Status status;
	[Export] DayOver dayOver;
	[Export] NewCardSelection newCardSelection;
	[Export] Button button;
	[Export] DeckViewUI deckViewUI;

	public override void _Ready()
	{
		base._Ready();
		if (status != null) {
			status.setLevel(global.currentLevel);
		}
		if (button!= null) {
			button.Pressed += ()=> deckViewUI.setUp(getDeckList());
		}

	}
	public override void advanceLevel() {
		GetTree().ChangeSceneToFile("res://mainScenes/game.tscn");
	}

	public void selectNewCard() {
		int cardsToChoose = getNumberOfCardToChoose();
		List<CardResource> cardPoolList = new List<CardResource>(cardPool.getCards());
		RandomHelper.Shuffle(cardPoolList);
		newCardSelection.setCardsToSelectFrom(cardPoolList.GetRange(0,cardsToChoose));
		newCardSelection.setCoins(0);
	}

}
