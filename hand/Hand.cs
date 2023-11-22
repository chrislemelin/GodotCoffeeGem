using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Hand : Node
{

	[Export] PackedScene cardTemplate;
	[Export] Node cardContainer;
	[Export] Discard discard;
	[Export] MatchBoard matchBoard;
	[Export] Deck deck;
	[Export] Sprite2D sprite2D;
	[Export] HandLine handLine;
	[Export] Mana mana;

	List<Card> cards = new List<Card>();

	Optional<Card> cardSelected = Optional.None<Card>();

	int handSize = 5;

	int width = 400;

	public Optional<Card> getCardSelected() {
		return cardSelected;
	}

	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		handLine.turnOff();
		newTurn();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	private void playCard(MatchBoard board, Card card, List<Vector2> positions) {
		card.playCard(board, mana, positions);
		mana.modifyMana(card.getCardResource().Cost * -1);
		clearSelectedCard();
		discardCard(card);
	}

	
	public void tilesSelectedForCard(MatchBoard board, List<Vector2> positions) {
		if(cardSelected.HasValue) {
			Card card = cardSelected.GetValue();
			playCard(board, card, positions);
		}
	}

	public bool hasRoomForMoreCards()
	{
		return cards.Count < handSize;
	}

	public bool addNewCardToHand(CardResource cardResource)
	{
		if (cards.Count == handSize)
		{
			return false;
		}
		Card newcard = cardTemplate.Instantiate() as Card;
		newcard.setUpCard(cardResource);
		newcard.area2D.InputEvent += (viewPort, inputEvent, temp) => cardClicked(inputEvent, newcard);

		AddChild(newcard);
		cards.Add(newcard);

		Vector2 startingPosition = getPositionForCard(cards.Count - 1);
		newcard.Position = startingPosition + new Vector2(0, 200);

		repositionCards();
		return true;
	}

	private void cardClicked(InputEvent inputEvent, Card card)
	{
		if (inputEvent.IsActionPressed("click"))
		{
			switch(card.getCardResource().getSelectionType()) {
				case SelectionType.Single:
				case SelectionType.Double:
					setSelectedCard(card);
					break;
				case SelectionType.None:
					clearSelectedCard();
					break;
			}
		};
	}

	private void setSelectedCard(Card card) {
		if (mana.manaValue >= card.getCardResource().Cost) {
			clearSelectedCard();
			handLine.turnOn(card.Position);
			cardSelected = Optional.Some<Card>(card);
			card.highlightOnHover.setForceHighlight(true);
		}
	}

	private void clearSelectedCard() {
		handLine.turnOff();
		matchBoard.clearTilesSelected();
		if (cardSelected.HasValue)
		{
			cardSelected.GetValue().highlightOnHover.setForceHighlight(false);
			cardSelected = Optional.None<Card>();
		}
	}

	public void newTurn () {
		discardHand();
		clearSelectedCard();
		mana.resetManaValue();
		deck.drawCards(5);
	}

	public void discardCard(Card card)
	{
		cards.Remove(card);
		discard.addCard(card.getCardResource());
		card.QueueFree();
		repositionCards();
	}

	public void discardHand() {
		foreach(Card card in cards) {
			discard.addCard(card.getCardResource());
			card.QueueFree();
		}
		cards.Clear();
	}

	public void repositionCards()
	{
		for (int index = 0; index < cards.Count; index++)
		{
			Node card = cards[index];
			lerp lerpScript = card as lerp;
			Vector2 sendingCardToVector = getPositionForCard(index);
			lerpScript.moveToPostion(sendingCardToVector);
		}
	}

	private Vector2 getPositionForCard(int index)
	{
		float newX = Mathf.Round((float)index * width / handSize * 2.0f - (width * (cards.Count - 1) / handSize));
		return new Vector2(newX, 0);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("right click")){
			if (@event is InputEventMouseButton eventMouseButton)
			{
				Vector2 posToCheck = sprite2D.ToLocal( eventMouseButton.Position);
				if (!sprite2D.GetRect().HasPoint(posToCheck)) {
					if (cardSelected.HasValue) {
						clearSelectedCard();
					}
				}

			}
		}
		if (@event.IsActionPressed("space")){
			newTurn();
		}
	}
}
