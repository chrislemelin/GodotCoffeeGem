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

	[Export] Array<CardResource> cards = new Array<CardResource>();
	[Export] int coinsGained = 0;

	public override void _Ready()
	{
		skipButton.Pressed += () => gameManager.advanceLevel();
		renderCards();
		renderCoins();
	}

	public void setCoins(int coinsGained) {
		this.coinsGained = coinsGained;
		renderCoins();
	}

	private void renderCoins() {
		if(coinsGained == 0) {
			foreach(Control control in levelPassedText) {
				control.Visible = false;
			}
		} else {
			foreach(Control control in levelPassedText) {
				control.Visible = true;
			}
			coinsGainedLabel.Text = coinsGainedLabel.Text + " " + coinsGained;
		}
	}

	public void setCardsToSelectFrom(List<CardResource> cardResources) {
		cards = new Array<CardResource>(cardResources);
		renderCards();
	}
	private void renderCards() {
		if (cards.Count > 0) {
			Visible = true;
			foreach(CardResource cardResource in cards) {
				CardInfoLoader cardInfoLoader = cardUIPackagedScene.Instantiate() as CardInfoLoader;
				cardInfoLoader.setUpCard(cardResource);
				cardContainer.AddChild(cardInfoLoader);
				cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource);
			}
		}
	}

	private void cardClicked(InputEvent inputEvent, CardResource cardResource) {
		if (inputEvent.IsActionPressed("click"))
		{
			gameManager.addCardToGlobalDeckAndAdvanceLevel(cardResource);
		};
	}

}
