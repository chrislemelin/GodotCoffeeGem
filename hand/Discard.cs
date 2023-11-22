using Godot;
using System;
using System.Collections.Generic;

public partial class Discard : Node
{	
	[Export] RichTextLabel countLabel;

	List<CardResource> cards = new List<CardResource>();

	public override void _Ready()
	{
		updateCount();
	}

	public void addCard(CardResource cardResource) {
		cards.Add(cardResource);
		updateCount();

	}

	public List<CardResource> getAllCardsAndReset() {
		List<CardResource> cardsTemp = new List<CardResource>(cards);
		cards.Clear();
		updateCount();
		return cardsTemp;
	}

	private void updateCount() {
		countLabel.Text = TextHelper.centered(cards.Count.ToString());
	}

}
