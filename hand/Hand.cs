using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Hand : Node
{

	[Export] PackedScene cardTemplate;
	[Export] Node2D cardContainer;
	[Export] Discard discard;
	[Export] MatchBoard matchBoard;
	[Export] Control cardSelectionUI;
	[Export] Deck deck;
	[Export] Sprite2D sprite2D;
	[Export] HandLine handLine;
	[Export] Mana mana;
	[Export] AudioStreamPlayer2D audioStreamPlayer2D;
	[Export] AudioStream newHandSoundEffect;
	[Export] AudioStream cardPlayedSoundEffect;
	[Export] GameManager gameManager;
	[Export] RelicResource relicResource;

	List<CardIF> cards = new List<CardIF>();
	List<CardResource> cardsPlayedThisTurn = new List<CardResource>();

	List<HandPassive> handPassives = new List<HandPassive>();


	Optional<CardIF> cardSelected = Optional.None<CardIF>();
	Optional<CardIF> cardHovered = Optional.None<CardIF>();

	int handSize = 10;
	int cardsDrawnOnNewTurn = 5;
	bool selectingCards = false;

	[Export]
	int width = 400;

	public Optional<CardIF> getCardSelected()
	{
		return cardSelected;
	}

	public Optional<CardIF> getCardHovered()
	{
		return cardHovered;
	}

	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		//gameManager.addRelic(relicResource);
		handPassives = gameManager.getRelics().SelectMany(passive => passive.getStartPassives<HandPassive>()).ToList();
		handLine.turnOff();
		setUpNewTurn();
		startNewTurn();
		mana.ManaChanged += () => checkCardsForDisabeling();
		FindObjectHelper.getNewTurnButton(this).SetUpNewTurn += () => setUpNewTurn();
		FindObjectHelper.getNewTurnButton(this).SetUpNewTurn += () => startNewTurn();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private int getCardsDrawnOnTurnStart()
	{
		int value = cardsDrawnOnNewTurn;
		foreach (HandPassive handPassive in handPassives)
		{
			value += handPassive.cardsOnTurnStart;
		}
		return value;
	}

	private void playCard(CardIF card, List<Vector2> positions)
	{
		if (mana.manaValue >= card.getCardResource().getCost())
		{
			card.playCard(matchBoard, this, mana, positions);
			cardsPlayedThisTurn.Add(card.getCardResource());
			audioStreamPlayer2D.Stream = cardPlayedSoundEffect;
			audioStreamPlayer2D.Play();
			mana.modifyMana(card.getCardResource().getCost() * -1);
			clearSelectedCard();
			discardCard(card);
			card.getCardResource().cardEffect.turnOver();
			checkCardsForDisabeling();
		}
	}


	public void tilesSelectedForCard(MatchBoard board, List<Vector2> positions)
	{
		if (cardSelected.HasValue)
		{
			CardIF card = cardSelected.GetValue();
			playCard(card, positions);
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
		CardIF newcard = cardTemplate.Instantiate() as CardIF;
		newcard.setCardResource(cardResource);
		newcard.clickedSignal += (inputEvent) => cardClicked(inputEvent, newcard);
		newcard.listenToMouseEnter(() => setCardHovered(newcard));
		newcard.listenToMouseExit(() => clearCardHovered());


		cardContainer.AddChild(newcard);
		cards.Add(newcard);

		Vector2 startingPosition = getPositionForCard(cards.Count - 1);
		newcard.Position = startingPosition + new Vector2(0, 200);

		repositionCards();
		checkCardsForDisabeling();
		return true;
	}

	public void cardsToSelect(int numberOfCards, bool required, CardEffectIF cardEffect)
	{

	}

	private void cardClicked(InputEvent inputEvent, CardIF card)
	{
		if (inputEvent.IsActionPressed("click"))
		{
			switch (card.getCardResource().getSelectionType())
			{
				case SelectionType.Single:
				case SelectionType.Double:
					setSelectedCard(card);
					break;
				case SelectionType.None:
					playCard(card, new List<Vector2>());
					clearSelectedCard();
					break;
			}
		};
	}

	private void setSelectedCard(CardIF card)
	{
		if (mana.manaValue >= card.getCardResource().getCost())
		{
			clearSelectedCard();
			handLine.turnOn(card.Position);
			cardSelected = Optional.Some<CardIF>(card);
			card.highlightOnHover.setForceHighlight(true);
		}
	}

	public List<CardIF> getAllCards()
	{
		return cards;
	}

	private void clearSelectedCard()
	{
		handLine.turnOff();
		matchBoard.clearTilesSelected();
		if (cardSelected.HasValue)
		{
			cardSelected.GetValue().highlightOnHover.setForceHighlight(false);
			cardSelected = Optional.None<CardIF>();
		}
	}

	public void checkCardsForDisabeling()
	{
		foreach (CardIF card in cards)
		{
			if (card.getCardResource().getCost() > mana.manaValue)
			{
				card.setDisabled();
			}
			else
			{
				card.setEnabled();
			}
		}
	}

	private void setCardHovered(CardIF card)
	{
		setCardHovered(Optional.Some<CardIF>(card));
	}

	private void clearCardHovered()
	{
		setCardHovered(Optional.None<CardIF>());
	}


	private void setCardHovered(Optional<CardIF> card)
	{
		if (cardHovered.HasValue && IsInstanceValid(cardHovered.GetValue()))
		{
			cardHovered.GetValue().moveToPostion(getPositionForCard(cardHovered.GetValue()));
		}
		cardHovered = card;
		if (cardHovered.HasValue)
		{
			cardContainer.MoveChild(cardHovered.GetValue(), cards.Count);
			cardHovered.GetValue().moveToPostion(getPositionForCard(cardHovered.GetValue()) - Vector2.Down * 25);
		}
		else
		{
			int count = 0;
			foreach (CardIF currentCard in cards)
			{
				cardContainer.MoveChild(currentCard, count);
				count++;
			}
		}
	}

	public void setUpNewTurn()
	{
		discardHand();
		cardsPlayedThisTurn.Clear();
		clearSelectedCard();
		mana.resetManaValue();
	}

	public void startNewTurn()
	{
		deck.drawCards(getCardsDrawnOnTurnStart());
		audioStreamPlayer2D.Stream = newHandSoundEffect;
		audioStreamPlayer2D.Play();
	}

	public void discardCard(CardIF card)
	{
		cards.Remove(card);
		discard.addCard(card.getCardResource());
		card.delete();
		repositionCards();
	}

	public void discardHand()
	{
		foreach (CardIF card in cards)
		{
			discard.addCard(card.getCardResource());
			card.delete();
		}
		cards.Clear();
	}

	public void repositionCards()
	{
		for (int index = 0; index < cards.Count; index++)
		{
			Node card = cards[index];
			lerp lerpScript = card as lerp;
			Vector2 sendingCardToVector = getPositionForCardV2(index);
			lerpScript.moveToPostion(sendingCardToVector);
		}
	}

	private Vector2 getPositionForCard(CardIF card)
	{
		for (int index = 0; index < cards.Count; index++)
		{
			CardIF currentCard = cards[index];
			lerp lerpScript = card as lerp;
			if (currentCard == card)
			{
				return getPositionForCardV2(index);
			}
		}
		return new Vector2(0, 0);
	}

	private Vector2 getPositionForCard(int index)
	{
		float newX = Mathf.Round((float)index * width / handSize * 2.0f - (width * (cards.Count - 1) / handSize));
		return new Vector2(newX, 0);
	}

	private Vector2 getPositionForCardV2(int index)
	{
		float newX = width * -0.5f + width / (cards.Count + 1) * (index + 1);
		return new Vector2(newX, 0);
	}

	public void drawCards(int count)
	{
		deck.drawCards(count);
		audioStreamPlayer2D.Stream = newHandSoundEffect;
		audioStreamPlayer2D.Play();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("right click"))
		{
			if (@event is InputEventMouseButton eventMouseButton)
			{
				if (cardSelected.HasValue)
				{
					clearSelectedCard();
				}
			}
		}
		if (@event.IsActionPressed("space"))
		{
			deck.drawCards(1);
		}
	}
}
