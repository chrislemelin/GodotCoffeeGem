using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class NewCardSelection : Control
{
	[Export] PackedScene cardUIPackagedScene;
	[Export] Control cardContainer;
	[Export] Button skipButton;
	[Export] RichTextLabel coinsGainedLabel;
	[Export] Array<Control> levelPassedText = new Array<Control>();
	[Export] DeckViewUI deckViewUI;
	[Export] Button viewDeckButton;

	[Export] Array<CardResource> cards = new Array<CardResource>();
	[Export] int coinsGained = 0;
	[Signal] public delegate void windowClosedEventHandler(CardResource cardResource);

	public override void _Ready()
	{
		renderCards();
		renderCoins();
		skipButton.Pressed += () => advance();
		viewDeckButton.Pressed += () => deckViewUI.setUp(FindObjectHelper.getGameManager(this).getDeckList());
	}

	public void setCoins(int coinsGained)
	{
		this.coinsGained = coinsGained;
		renderCoins();
	}

	private void renderCoins()
	{
		if (coinsGained == 0)
		{
			foreach (Control control in levelPassedText)
			{
				control.Visible = false;
			}
		}
		else
		{
			foreach (Control control in levelPassedText)
			{
				control.Visible = true;
			}
			coinsGainedLabel.Text = coinsGainedLabel.Text + " " + coinsGained;
		}
	}

	public void getRandomCardsToSelectFrom()
	{
		GameManagerIF gameManager = FindObjectHelper.getGameManager(this);
		int numberOfCardsToChoose = gameManager.getNumberOfCardToChoose();
		setCardsToSelectFrom(CardRarityHelper.getRandomCards(numberOfCardsToChoose, gameManager.cardPool));
	}

	public void setCardsToSelectFrom(List<CardResource> cardResources)
	{
		cards = new Array<CardResource>(cardResources);
		renderCards();
	}
	private void renderCards()
	{
		if (cards.Count > 0)
		{
			Visible = true;
			foreach (Node child in cardContainer.GetChildren())
			{
				child.QueueFree();
			};
			foreach (CardResource cardResource in cards)
			{
				CardInfoLoader cardInfoLoader = cardUIPackagedScene.Instantiate() as CardInfoLoader;
				cardInfoLoader.setUpCard(cardResource);
				cardContainer.AddChild(cardInfoLoader);
				cardInfoLoader.flipCard();
				cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource);
			}
		}
	}

	private void cardClicked(InputEvent inputEvent, CardResource cardResource)
	{
		if (inputEvent.IsActionPressed("click"))
		{
			FindObjectHelper.getGameManager(this).addCardToDeckList(cardResource);
			advance(cardResource);
		};
	}

	private void advance(CardResource cardResource)
	{
		Visible = false;
		EmitSignal(SignalName.windowClosed, cardResource);
	}

	private void advance()
	{
		Visible = false;

		EmitSignal(SignalName.windowClosed, new CardResource());
	}
}
