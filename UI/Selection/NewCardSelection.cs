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
	[Export] Array<CardResource> cardsToTest = new Array<CardResource>();
	[Export] ButtonWithCoinCost refreshSelectionButton;

	private GameManagerIF gameManagerIF;

	private bool cardSelected = false;

	public override void _Ready()
	{
		// renderCards();
		renderCoins();
		skipButton.Pressed += () => advance();
		gameManagerIF = FindObjectHelper.getGameManager(this);
		viewDeckButton.Pressed += () => deckViewUI.setUp(gameManagerIF.getDeckList());
		refreshSelectionButton.Pressed += () =>  {
			gameManagerIF.addCoins(- refreshSelectionButton.cost);
			getRandomCardsToSelectFrom();
		};
		gameManagerIF.coinsChanged += (int value) => checkIfButtonIsActivated();
		checkIfButtonIsActivated();
	}

	public void checkIfButtonIsActivated() {
		if (gameManagerIF.getCoins() >= refreshSelectionButton.cost) {
			refreshSelectionButton.Disabled = false;
		} else {
			refreshSelectionButton.Disabled = true;
		}
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
		cards.AddRange(cardsToTest);
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
				cardContainer.AddChild(cardInfoLoader);
				cardInfoLoaders.Add(cardInfoLoader);
				cardInfoLoader.setUpCard(cardResource);
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
