using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Deck : Node
{
	[Export] RichTextLabel countLabel;
	[Export] Hand hand;
	[Export] CardResource[] allCards;
	[Export] Discard discard;
	List<CardResource> cards = new List<CardResource>();

	public override void _Ready()
	{
		addCardsToDeck();
		RandomHelper.Shuffle(cards);
		updateCount();
	}

	private void addCardsToDeck() {
		while(cards.Count < 10) {
			cards.AddRange(allCards);
		}
	}

	public void drawCards(int count)
	{
		for (int a = 0; a < count; a++)
		{
			drawCard();
		}
	}

	private void updateCount()
	{
		countLabel.Text = TextHelper.centered(cards.Count.ToString());
	}

	private void drawCard()
	{
		if (cards.Count == 0)
		{
			cards.AddRange(discard.getAllCardsAndReset());
			updateCount();
			if (cards.Count == 0) {
				return;
			}

		}
		CardResource nextCard = cards[0];
		bool cardAddedToHand = hand.addNewCardToHand(nextCard);
		if (cardAddedToHand) {
			cards.RemoveAt(0);
			updateCount();
		}

	}
	public override void _Input(InputEvent @event)
	{}


}
