using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Deck : Node
{
	[Export] RichTextLabel countLabel;
	[Export] Hand hand;
	[Export] CardResource[] allCards;
	List<CardResource> cards = new List<CardResource>();

	public override void _Ready()
	{
		cards.AddRange(allCards);
		RandomHelper.Shuffle(cards);
		updateCount();
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
			//shuffle from discard
			return;
		}
		CardResource nextCard = cards[0];
		cards.RemoveAt(0);
		hand.addNewCardToHand(nextCard);
		updateCount();
	}
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("click"))
		{
			drawCard();
		}
		if (@event.IsActionPressed("right click"))
		{
			//deleteCard();
		}
	}


}
