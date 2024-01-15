using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Deck : Node
{
	[Export] RichTextLabel countLabel;
	[Export] Hand hand;
	[Export] Godot.Collections.Array<CardResource> allCards;
	[Export] CardList cardList;
	[Export] Discard discard;
	[Export] Boolean loadDeckFromGlobal;
	[Export] GameManager gameManager;
	[Export] DeckViewUI deckView;
	[Export] Control control;
	List<CardResource> cards = new List<CardResource>();

	public override void _Ready()
	{
		if (loadDeckFromGlobal) {
			List<CardResource> deckCardList = gameManager.getDeckList();
			if (deckCardList.Count != 0) {
				foreach (CardResource cardResource in deckCardList) {
 					allCards.Add((CardResource)cardResource.Duplicate());
				}
				
			}
		}

		control.GuiInput += (inputEvent) =>  {
			if (inputEvent.IsActionPressed("click")) {
				deckView.setUp(cards);
			}	
		}; 

		addCardsToDeck();
		RandomHelper.Shuffle(cards);
		updateCount();
		GetNode<Button>(FindObjectHelper.NEW_BUTTON_NAME).Pressed += () => turnOver();
	}

	private void addCardsToDeck() {
		cards.AddRange(allCards);
	}

	private void turnOver() {
		foreach(CardResource cardResource in allCards) {
			cardResource.cardEffect.turnOver();
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
