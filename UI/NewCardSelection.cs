using Godot;
using System;
using System.Collections.Generic;

public partial class NewCardSelection : Control
{
	[Export] PackedScene cardUIPackagedScene;
	[Export] Control cardContainer;
	[Export] GameManager gameManager;


	public override void _Ready()
	{
		//Visible = false;
	}

	public void setCardsToSelectFrom(List<CardResource> cardResources) {
		Visible = true;
		foreach(CardResource cardResource in cardResources) {
			CardInfoLoader cardInfoLoader = cardUIPackagedScene.Instantiate() as CardInfoLoader;
			cardInfoLoader.setUpCard(cardResource);
			cardContainer.AddChild(cardInfoLoader);
			cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource);
		}
	}

	private void cardClicked(InputEvent inputEvent, CardResource cardResource) {
		if (inputEvent.IsActionPressed("click"))
		{
			gameManager.addCardToGlobalDeck(cardResource);
			Visible = false;
		};
	}

}
