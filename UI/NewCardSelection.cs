using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class NewCardSelection : Control
{
	[Export] PackedScene cardUIPackagedScene;
	[Export] Control cardContainer;
	[Export] GameManagerIF gameManager;
	[Export] Button skipButton;
	[Export] RichTextLabel coinsGainedLabel;
	[Export] Array<Control> levelPassedText = new Array<Control>(); 

	public override void _Ready()
	{
		skipButton.Pressed += () => gameManager.advanceLevel();
	}

	public void setCoins(int coinsGained) {
		if(coinsGained == 0) {
			foreach(Control control in levelPassedText) {
				control.Visible = false;
			}
		} else {
			coinsGainedLabel.Text = coinsGainedLabel.Text + " " + coinsGained;
		}
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
			gameManager.addCardToGlobalDeckAndAdvanceLevel(cardResource);
		};
	}

}
