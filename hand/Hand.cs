using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Hand : ControllerInput
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
	[Export] AudioStreamPlayer2D cardSelectedaudioStreamPlayer2D;
	[Export] AudioStreamPlayer2D cardHoveraudioStreamPlayer2D;

	[Export] AudioStream newHandSoundEffect;
	[Export] AudioStream cardPlayedSoundEffect;
	[Export] AudioStream cardSelectedSoundEffect;

	[Export] GameManager gameManager;
	[Export] RelicResource relicResource;
	[Signal] public delegate void cardDrawnEventHandler(CardResource cardResource);
	[Signal] public delegate void cardDrawnNotFromNewTurnEventHandler(CardResource cardResource);
	[Signal] public delegate void cardPlayedEventHandler(CardResource cardResource);
	[Signal] public delegate void cardAboutToPlayEventHandler(CardResource cardResource);
	[Signal] public delegate void handChangedEventHandler();


	List<CardIF> cards = new List<CardIF>();
	List<CardResource> cardsPlayedThisTurn = new List<CardResource>();
	List<HandPassive> handPassives = new List<HandPassive>();
	Optional<CardIF> cardSelected = Optional.None<CardIF>();
	private int hoveredCardIndex = 0;
	[Export] private bool hasUIFocus = true;
	[Export] public bool forceNotHaveUIFocus = false;

	[Export] Godot.Collections.Array<Control> windowsInFrontOf;
	private bool hitDeadZone = true;
	Optional<CardIF> cardHovered = Optional.None<CardIF>();
	Optional<int> tutorialCardMustPlay = Optional.None<int>();
	private bool drawnFirstHand = false;

	int handSize = 10;
	int cardsDrawnOnNewTurn = 5;
	bool selectingCards = false;

	[Export]
	int width = 400;
	[Export] float hoverUpDistance = 90;

	public void modifyNewCardsDrawnOnNewTurn(int value) {
		cardsDrawnOnNewTurn += value;
	}

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
		ControllerHelper controllerHelper = FindObjectHelper.getControllerHelper(this);
		hasUIFocus = controllerHelper.isUsingController();
		controllerHelper.UsingControllerChanged += setUIFocus;
		FindObjectHelper.getSettingsMenu(this).windowOpened += () => setUIFocus(false);
		gameManager.levelStart += () => startLevel();
		gameManager.levelOver += () => setUIFocus(false);
		mana.ManaChanged += () => checkCardsForDisabeling();
		handLine.turnOff();
		matchBoard.ingredientMatched += (match) => checkCardsForDisabeling();
	}

	private void startLevel(){
		//cleanUpOldTurn();
		startNewTurn();
		FindObjectHelper.getNewTurnButton(this).TurnCleanUp += () => cleanUpOldTurn();
		FindObjectHelper.getNewTurnButton(this).StartNewTurn += () => startNewTurn();
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
		if (mana.manaValue >= card.getCardResource().getEnergyCost() && card.getEnabled())
		{
			EmitSignal(SignalName.cardAboutToPlay, card.getCardResource());
			discardCard(card, true);
			int manaCost = card.getCardResource().getEnergyCost();
			if (card.getCardResource().cardEffect.spendX) {
				manaCost = mana.manaValue;
				card.getCardResource().cardEffect.spendXValue = manaCost;
			}

			card.playCard(matchBoard, this, mana, positions);
			tutorialCardMustPlay = Optional.None<int>();
			EmitSignal(SignalName.cardPlayed, card.getCardResource());
			cardsPlayedThisTurn.Add(card.getCardResource());
			audioStreamPlayer2D.Stream = cardPlayedSoundEffect;
			audioStreamPlayer2D.Play();

			mana.modifyMana(manaCost* -1);
			clearSelectedCard();
			card.getCardResource().cardEffect.turnOver();
			checkCardsForDisabeling();
			changeHoveredCard(0);
			EmitSignal(SignalName.handChanged);
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

	public bool addNewCardToHand(CardResource cardResource, bool fromNewTurn)
	{
		if (cards.Count == handSize)
		{
			return false;
		}
		CardIF newcard = cardTemplate.Instantiate() as CardIF;
		cardContainer.AddChild(newcard);
		newcard.setCardResource(cardResource);
		newcard.clickedSignal += (inputEvent) => cardClicked(inputEvent, newcard);
		newcard.listenToMouseEnter(() => setCardHovered(newcard));
		newcard.listenToMouseExit(() => clearCardHovered());
		cards.Add(newcard);

		Vector2 startingPosition = getPositionForCardV2(cards.Count - 1);
		newcard.Position = startingPosition + new Vector2(0, 200);

		repositionCards();
		checkCardsForDisabeling();
		EmitSignal(SignalName.cardDrawn, cardResource);
		if (!fromNewTurn){
			EmitSignal(SignalName.cardDrawnNotFromNewTurn, cardResource);
			EmitSignal(SignalName.handChanged);
		}
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

	private bool setSelectedCard(CardIF card)
	{
		if (mana.manaValue >= card.getCardResource().getEnergyCost() && card.getEnabled())
		{
			playCardSelectedSound();
			clearSelectedCard();
			handLine.turnOn(card.Position);
			cardSelected = Optional.Some<CardIF>(card);
			card.highlightOnHover.setForceHighlightAltColor(true);
			return true;
		}
		return false;
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
		int index = 0;
		foreach (CardIF card in cards)
		{
			// if (card.getCardResource().getEnergyCost() > mana.manaValue || !card.getCardResource().canPlayCard())
			// {
			// 	card.setDisabled();
			// }
			// else
			// {
			// 	card.setEnabled();
			// }
			if (tutorialCardMustPlay.HasValue) {
				if (index == tutorialCardMustPlay.GetValue()) {
					card.setEnabled();
				} else {
					card.setDisabledForTutorial();
				}
			}
			index++;
		}
	}

	public void disableAllCardsBut(int index) {
		tutorialCardMustPlay = Optional.Some<int>(index);
		checkCardsForDisabeling();
	}

	private void setCardHovered(CardIF card)
	{
		setCardHovered(Optional.Some<CardIF>(card));
		cardHoveraudioStreamPlayer2D.Play();
	}

	private void clearCardHovered()
	{
		setCardHovered(Optional.None<CardIF>());
	}

	public bool hasPlayableCards() {
		foreach(CardIF card in cards){
			if (card.getEnabled()) {
				return true;
			}
		}
		return false;
	}


	private void setCardHovered(Optional<CardIF> card)
	{
		if(card.HasValue && cardHovered.HasValue && card.GetValue() == cardHovered.GetValue()) {
			return;
		} 

		if (cardHovered.HasValue && IsInstanceValid(cardHovered.GetValue()))
		{
			cardHovered.GetValue().moveToPostion(getPositionForCard(cardHovered.GetValue()));
			cardHovered.GetValue().highlightOnHover.setForceHighlight(false);
		}
		cardHovered = card;
		if (cardHovered.HasValue)
		{
			cardContainer.MoveChild(cardHovered.GetValue(), cards.Count);
			cardHovered.GetValue().moveToPostion(getPositionForCard(cardHovered.GetValue()) - Vector2.Down * hoverUpDistance);
			cardHovered.GetValue().highlightOnHover.setForceHighlight(true);
		}
		else
		{
			int count = 0;
			foreach (CardIF currentCard in cards)
			{
				Callable.From(() => cardContainer.MoveChild(currentCard, count)).CallDeferred();
				count++;
			}
		}
	}

	public void cleanUpOldTurn()
	{
		clearSelectedCard();
		discardHand();
		cardsPlayedThisTurn.Clear();
		mana.resetManaValue();
	}

	public void startNewTurn()
	{
		drawCardsFromNewTurn(getCardsDrawnOnTurnStart());
		audioStreamPlayer2D.Stream = newHandSoundEffect;
		audioStreamPlayer2D.Play();
		if (hasUIFocus) {
			setCardHovered(cards[0]);
		}
		setUIFocus(true);
	}

	public void addWindowInFrontOf(Control control) {
		windowsInFrontOf.Add(control);
	}
	public void setUIFocus(bool value) {
		if(FindObjectHelper.getControllerHelper(this).isUsingController() 
			&& !FindObjectHelper.getScore(this).isLevelOver() 
			&& !windowsInFrontOf.Where(control => control.IsVisibleInTree()).Any() 
			&& !FindObjectHelper.getSettingsMenu(this).isVisible() 
			&& !forceNotHaveUIFocus) {
			hasUIFocus = value;
			if (hasUIFocus) {
				changeHoveredCard(0);
			} else {
				clearCardHovered();
			}
		} else {
			hasUIFocus = false;
			clearCardHovered();
		}
		
	}

	public void discardCard(CardIF card, bool playingCard, bool keepRetain = true)
	{
		if (!card.getCardResource().cardEffect.retain || playingCard || !keepRetain) {
			cards.Remove(card);
			card.delete();
			if (!card.getCardResource().cardEffect.consume || !playingCard) {
				discard.addCard(card.getCardResource());
			}
			repositionCards();
		}
	}

	public void discardHand(bool keepRetain = true)
	{
		foreach (CardIF card in cards.ToList())
		{
			discardCard(card, false, keepRetain);
			// discard.addCard(card.getCardResource());
			// card.delete();
		}
		//cards.Clear();
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
		drawCards(count, false);
	}

	private void drawCards(int count, bool fromNewTurn)
	{
		if (!drawnFirstHand) {
			int cardsDrawn = deck.drawInnateCards(count, fromNewTurn);
			count -= cardsDrawn;
		}
		deck.drawCards(count, fromNewTurn);
		audioStreamPlayer2D.Stream = newHandSoundEffect;
		audioStreamPlayer2D.Play();
	}

	private void drawCardsFromNewTurn(int count) {
		drawCards(count, true);
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
			//deck.drawCards(1, false);
		}

		if(@event.IsActionPressed("ui_accept") && hasUIFocus)
		{
			if(cardHovered.HasValue) {
				CardIF card = cardHovered.GetValue();
				switch (card.getCardResource().getSelectionType())
				{
					case SelectionType.Single:
					case SelectionType.Double:
						bool selected = setSelectedCard(card);
						if (selected) {
							GetTree().CreateTimer(.1f).Timeout+= () => matchBoard.setUIFocus(true);
							hasUIFocus = false;
						}
						break;
					case SelectionType.None:
						playCard(card, new List<Vector2>());
						clearSelectedCard();
						break;
				}
			}
		}
		if(@event.IsActionPressed("ui_cancel") && !hasUIFocus && matchBoard.getUIFocus())
		{
			clearSelectedCard();
			matchBoard.setUIFocus(false);
			this.setUIFocus(true);
		}
		base._Input(@event);
	}


	protected override void controllerLeft() {
		if(hasUIFocus) {
			changeHoveredCard(-1);
		}
	}
	protected override void controllerRight() {
		if(hasUIFocus) {
			changeHoveredCard(1);
		}
	}

	private void changeHoveredCard(int value) {
		if (hasUIFocus) {
			hoveredCardIndex = Math.Clamp(value + hoveredCardIndex, 0, cards.Count-1);
			setCardHovered(cards[hoveredCardIndex]);
		}
	}

	private void playCardSelectedSound() {
		cardSelectedaudioStreamPlayer2D.Play();
	}

	public override void _ExitTree()
	{
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged -= setUIFocus;
	}
}
