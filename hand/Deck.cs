using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Deck : DeckTutorial
{
	[Export] RichTextLabel countLabel;
	[Export] Hand hand;
	[Export] Godot.Collections.Array<CardResource> allCards = new Godot.Collections.Array<CardResource>();
	[Export] CardList cardList;
	[Export] Discard discard;
	[Export] Boolean loadDeckFromGlobal;
	[Export] GameManager gameManager;
	[Export] DeckViewUI deckView;
	[Export] Control control;
	[Export] Sprite2D background;

	List<CardResource> cards = new List<CardResource>();
	private int cardDrawsTutorial = 0;


	public override void _Ready()
	{
		if (loadDeckFromGlobal)
		{
			allCards.Clear();
			List<CardResource> deckCardList = gameManager.getDeckList();
			if (tutorial) {
				deckCardList = tutorialCards.getCards();
			} 
			if (deckCardList.Count != 0)
			{
				foreach (CardResource cardResource in deckCardList)
				{
					CardResource cardResourceToAdd = cardResource.duplicate();
					allCards.Add(cardResourceToAdd);
				}
			}
		}

		control.GuiInput += (inputEvent) =>
		{
			if (inputEvent.IsActionPressed("click"))
			{
				deckView.setUp(cards);
			}
		};

		addCardsToDeck();
		if (!tutorial) {
			RandomHelper.Shuffle(cards);
		}
		updateCount();
		FindObjectHelper.getNewTurnButton(this).Pressed += () => turnOver();

		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
		if (gameManagerIF.getDeckSelection() != null) {
			background.Texture = gameManagerIF.getDeckSelection().faceCard;
		}
	}

	private void addCardsToDeck()
	{
		cards.AddRange(allCards);
	}

	private void turnOver()
	{
		foreach (CardResource cardResource in allCards)
		{
			cardResource.cardEffect.turnOver();
		}
	}

	public int drawInnateCards(int count, bool fromNewTurn)
	{
		List<CardResource> innateCards = getInnateCards();
		int cardsAdded = 0;
		foreach(CardResource cardResource in innateCards) {
			if (cardsAdded == count) {
				return cardsAdded;
			}
			drawSpecificCard(cardResource, fromNewTurn);
			cardsAdded ++;
		}
		return cardsAdded;
	}

	private List<CardResource> getInnateCards() {
		return cards.Where(card => card.cardEffect.innate).ToList();
	}


	public void drawCards(int count, bool fromNewTurn)
	{
		if (tutorial && fromNewTurn) {
			cardDrawsTutorial ++;
			if (cardDrawsTutorial == 1) {
				count = 1;
			} if (cardDrawsTutorial == 2) {
				count = 5;
			}
		}
		for (int a = 0; a < count; a++)
		{
			drawCard(fromNewTurn);
		}
	}

	private void updateCount()
	{
		countLabel.Text = TextHelper.centered(cards.Count.ToString());
	}

	private void drawCard(bool fromNewTurn)
	{
		if (cards.Count == 0)
		{
			cards.AddRange(discard.getAllCardsAndReset());
			RandomHelper.Shuffle(cards);
			updateCount();
			if (cards.Count == 0)
			{
				return;
			}

		}
		CardResource nextCard = cards[0];
		bool cardAddedToHand = hand.addNewCardToHand(nextCard, fromNewTurn);
		if (cardAddedToHand)
		{
			cards.RemoveAt(0);
			updateCount();
		} else {
			cards.RemoveAt(0);
			discard.addCard(nextCard);
		}

	}

	private void drawSpecificCard(CardResource cardResource, bool fromNewTurn) {
		cards.Remove(cardResource);
		hand.addNewCardToHand(cardResource, fromNewTurn);
	}
	public override void _Input(InputEvent @event)
	{ }


}
