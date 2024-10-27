using Godot;
using System;
using System.Collections.Generic;

public partial class Discard : Node
{	
	[Export] RichTextLabel countLabel;
	[Export] DeckViewUI deckView;
	[Export] Control control;
	[Export] Sprite2D background;
 
	List<CardResource> cards;
	public override void _Ready()
	{
		cards = new List<CardResource>();
		updateCount();
		control.GuiInput += (inputEvent) =>  {
			if (inputEvent.IsActionPressed("click") && cards.Count != 0) {
				deckView.setUp(cards);
			}	
		}; 
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
		if (gameManagerIF.getDeckSelection() != null) {
			background.Texture = gameManagerIF.getDeckSelection().faceCard;
		}
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
