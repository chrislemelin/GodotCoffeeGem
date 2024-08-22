using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Deck : Node
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
	List<CardResource> cards = new List<CardResource>();


	public override void _Ready()
	{
		if (loadDeckFromGlobal)
		{
			allCards.Clear();
			List<CardResource> deckCardList = gameManager.getDeckList();
			if (deckCardList.Count != 0)
			{
				foreach (CardResource cardResource in deckCardList)
				{
					CardResource cardResourceToAdd = (CardResource)cardResource.Duplicate();
					cardResourceToAdd.cardEffect = (CardEffectIF)cardResource.cardEffect.Duplicate();
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
		RandomHelper.Shuffle(cards);
		updateCount();
		FindObjectHelper.getNewTurnButton(this).Pressed += () => turnOver();
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

	public void drawCards(int count, bool fromNewTurn)
	{
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
	public override void _Input(InputEvent @event)
	{ }


}
