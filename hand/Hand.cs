using Godot;
using System;
using System.Collections.Generic;

public partial class Hand : Node
{

	[Export] PackedScene cardTemplate;
	[Export] Node cardContainer;


	List<Card> cards = new List<Card>();
	Optional<Card> cardSelected = Optional.None<Card>();

	int handSize = 5;

	int width = 400;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public bool hasRoomForMoreCards()
	{
		return cards.Count < handSize;
	}

	public void addNewCardToHand(CardResource cardResource)
	{
		if (cards.Count == handSize)
		{
			return;
		}
		Card newcard = cardTemplate.Instantiate() as Card;
		newcard.setUpCard(cardResource);
		newcard.area2D.InputEvent += (viewPort, inputEvent, temp) => cardClicked(inputEvent, newcard);

		AddChild(newcard);
		cards.Add(newcard);

		Vector2 startingPosition = getPositionForCard(cards.Count - 1);
		newcard.Position = startingPosition + new Vector2(0, 200);

		repositionCards();
	}

	private void cardClicked(InputEvent inputEvent, Card card)
	{
		if (inputEvent.IsActionPressed("click"))
		{
			if (cardSelected.HasValue)
			{
				cardSelected.GetValue().highlightOnHover.setForceHighlight(false);
			}
			cardSelected = Optional.Some<Card>(card);
			card.highlightOnHover.setForceHighlight(true);
		};
	}
	public void deleteCard()
	{
		if (cards.Count == 0)
		{
			return;
		}
		Card card = cards[cards.Count - 1];
		cards.Remove(card);
		card.QueueFree();
		repositionCards();
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
		if (@event.IsActionPressed("click"))
		{
			//addNewCardToHand();
		}
		if (@event.IsActionPressed("right click"))
		{
			deleteCard();
		}
	}
}
