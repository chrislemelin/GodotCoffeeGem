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
	List<CardInfoLoader> cardInfoLoaders = new List<CardInfoLoader>();
	[Signal] public delegate void windowClosedEventHandler(CardResource cardResource);
	[Export] AnimationPlayer animationPlayer;

	private bool cardSelected = false;

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
		cardSelected = false;
		cardInfoLoaders.Clear();
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
				cardInfoLoaders.Add(cardInfoLoader);
				cardInfoLoader.setUpCard(cardResource);
				cardContainer.AddChild(cardInfoLoader);
				cardInfoLoader.flipCard();
				cardInfoLoader.setForceHighlightOff(true);
				GetTree().CreateTimer(.25).Timeout += () => cardInfoLoader.setForceHighlightOff(false);
				cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardInfoLoader);
			}
		}
	}

	private void cardClicked(InputEvent inputEvent, CardInfoLoader cardInfoLoader)
	{
		if (inputEvent.IsActionPressed("click") && !cardSelected)
		{
			cardSelected = true;
			CardResource cardResource = cardInfoLoader.cardResource;
			FindObjectHelper.getGameManager(this).addCardToDeckList(cardResource);
			GetTree().CreateTimer(.75).Timeout += () => advance(cardResource);
			foreach(CardInfoLoader currentCardInfoLoader in cardInfoLoaders) {
				if (currentCardInfoLoader != cardInfoLoader) {
					currentCardInfoLoader.destroyCard();
				}
			}
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
