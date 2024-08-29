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
	[Export] CustomButton viewDeckButton;
	[Export] Array<CardResource> cards = new Array<CardResource>();
	[Export] int coinsGained = 0;
	List<CardInfoLoader> cardInfoLoaders = new List<CardInfoLoader>();
	[Signal] public delegate void windowClosedEventHandler(CardResource cardResource);
	[Export] AnimationPlayer animationPlayer;
	[Export] Array<CardResource> cardsToTest = new Array<CardResource>();
	[Export] AudioPlayer audioPlayer;
	[Export] ButtonWithCoinCost refreshSelectionButton;
	Action callback = null;

	private GameManagerIF gameManagerIF;

	private bool cardSelected = false;
	private bool otherWindowOpen = false;

	public override void _Ready()
	{
		// renderCards();
		Visible = false;
		renderCoins();
		skipButton.Pressed += () => advance();
		gameManagerIF = FindObjectHelper.getGameManager(this);
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged += setUIFocus;

		viewDeckButton.Pressed += () =>  {
			otherWindowOpen = true;
			deckViewUI.setUp(gameManagerIF.getDeckList());
		};
		deckViewUI.WindowClosedSignal += () => {
			otherWindowOpen = false;
			setUIFocus();
		};
		refreshSelectionButton.Pressed += () =>
		{
			gameManagerIF.addCoins(-refreshSelectionButton.cost);
			getRandomCardsToSelectFromInternal();
		};
		gameManagerIF.coinsChanged += (int value) => checkIfButtonIsActivated();
		checkIfButtonIsActivated();
	}

	public void checkIfButtonIsActivated()
	{
		if (gameManagerIF.getCoins() >= refreshSelectionButton.cost)
		{
			refreshSelectionButton.Disabled = false;
		}
		else
		{
			refreshSelectionButton.Disabled = true;
		}
	}

	public void setCoins(int coinsGained)
	{
		this.coinsGained = coinsGained;
		renderCoins();
	}

	public void setCallBack(Action callback)
	{
		this.callback = callback;
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
		getRandomCardsToSelectFromInternal();
		setUIFocus();
	}
	private void getRandomCardsToSelectFromInternal()
	{
		GameManagerIF gameManager = FindObjectHelper.getGameManager(this);
		int numberOfCardsToChoose = gameManager.getNumberOfCardToChoose();
		setCardsToSelectFrom(CardRarityHelper.getRandomCards(numberOfCardsToChoose, gameManager.getCardPool()));
	}

	public void setCardsToSelectFrom(List<CardResource> cardResources)
	{
		cards = new Array<CardResource>(cardResources);
		cards.AddRange(cardsToTest);
		GetTree().CreateTimer(.40f).Timeout += () =>
		{
			audioPlayer.Play();
		};

		renderCards();
	}

	private void setUIFocus() {
		setUIFocus(true);
	}

	private void setUIFocus(bool focused) {
		if(otherWindowOpen){
			return;
		}
		if(focused && IsVisibleInTree() && FindObjectHelper.getControllerHelper(this).isUsingController()) {
			if(cardInfoLoaders.Count > 0) {
				cardInfoLoaders[0].GrabFocus();
			}
		} else {
			foreach (CardInfoLoader cardInfoLoader in cardInfoLoaders){
				cardInfoLoader.ReleaseFocus();
			}
		}
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
			//setUIFocus();
		}
	}

	private void cardClicked(InputEvent inputEvent, CardInfoLoader cardInfoLoader)
	{
		if (InputHelper.isSelectedAction(inputEvent) && !cardSelected)
		{
			cardSelected = true;
			CardResource cardResource = cardInfoLoader.cardResource;
			FindObjectHelper.getGameManager(this).addCardToDeckList(cardResource);
			GetTree().CreateTimer(.75).Timeout += () => advance(cardResource);
			foreach (CardInfoLoader currentCardInfoLoader in cardInfoLoaders)
			{
				if (currentCardInfoLoader != cardInfoLoader)
				{
					currentCardInfoLoader.destroyCard();
				}
			}
		};
	}

	private void advance(CardResource cardResource)
	{
		Visible = false;
		if (callback != null)
		{
			callback.Invoke();
			callback = null;
		}
		EmitSignal(SignalName.windowClosed, cardResource);
	}

	private void advance()
	{
		Visible = false;
		if (callback != null)
		{
			callback.Invoke();
			callback = null;
		}
		EmitSignal(SignalName.windowClosed, new CardResource());
	}
	public override void _ExitTree()
	{
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged -= setUIFocus;
	}
}
