using Godot;
using System;
using System.Collections.Generic;
using System.Security.Authentication;

public partial class CardIF : lerp
{
	[Export] protected RichTextLabel titleLabel;
	[Export] protected RichTextLabel descriptionLabel;
	[Export] protected RichTextLabel costLabel;
	[Export] protected TextureRect picture;
	[Export] protected TextureRect background;

	[Export] public HighlightOnHover highlightOnHover;
	[Export] protected CardResource cardResource;
	[Export] protected Color disabledColor;
	[Export] protected TextureRect titleSprite;
	[Export] RichTextLabel toolTipText2;

	private bool enabled = true;
	private bool tutorialDisabled = false;
	Mana mana;


	[Signal]
	public delegate void clickedSignalEventHandler(InputEvent inputEvent);


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
		if (gameManagerIF.getDeckSelection() != null) {
			background.Texture = gameManagerIF.getDeckSelection().faceCardFront;
		}
	}

	public bool getEnabled()
	{
		return enabled && cardResource.canPlayCard();
	}

	public void setDisabledForTutorial()
	{
		enabled = false;
		tutorialDisabled = true;
		Modulate = disabledColor;
	}

	public void setDisabled()
	{
		enabled = false;
		Modulate = disabledColor;
	}
	public void setEnabled()
	{
		enabled = true;
		tutorialDisabled = false;
		Modulate = new Color(1, 1, 1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

	}

	public void setCardResource(CardResource cardResource)
	{
		mana = FindObjectHelper.getMana(this);
		if (this.cardResource != null)
		{
			cardResource.cardEffect.CardPassivesChanged -= setUpCard;
		}
		this.cardResource = cardResource;
		setUpCard();
		checkDisabled();

		cardResource.cardEffect.CardPassivesChanged += setUpCard;
		cardResource.cardEffect.ValueChanged += setUpCard;
		cardResource.cardEffect.CustomTextChanged += setUpCard;
		
		mana.ManaChanged += checkDisabled;
		cardResource.init();
	}

	private void checkDisabled() {
		if (cardResource.getEnergyCost() <= mana.manaValue && cardResource.canPlayCard() && !tutorialDisabled){
			setEnabled();
		} else {
			setDisabled();
		}

	}

	private void setUpCard()
	{
		cardResource.node = this;
		cardResource.cardEffect.node = this;
		if (cardResource.Picture != null)
		{
			picture.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			picture.Texture = cardResource.Picture;
		}

		titleLabel.Text = TextHelper.centered(cardResource.Title);
		descriptionLabel.Text = TextHelper.centered(cardResource.getDescription());
		costLabel.Text = TextHelper.centered(cardResource.getEnergyCostString());
		titleSprite.Modulate = cardResource.rarity.getColor();
		//highlightOnHover.TooltipText = cardResource.getToolTip();
		toolTipText2.Text = cardResource.getToolTip();
		checkDisabled();
	}

	public void playCard(MatchBoard matchboard, Hand hand, Mana mana, List<Vector2> tiles)
	{
		cardResource.cardEffect.doEffect(matchboard, hand, mana, tiles, Optional.None<CardIF>());
		FindObjectHelper.getGameManager(this).addCoins(-1 * cardResource.coinPlayCost);
	}

	public virtual void listenToMouseEnter(Action function)
	{
	}

	public virtual void listenToMouseExit(Action function)
	{
	}

	public void delete()
	{
		//cardResource.cardEffect.CardPassivesChanged -= setUpCard;
		QueueFree();
	}
	public CardResource getCardResource()
	{
		return cardResource;
	}
	protected override void Dispose(bool disposing)
	{
		if (cardResource != null)
		{
			cardResource.cardEffect.CardPassivesChanged -= setUpCard;
			cardResource.cardEffect.ValueChanged -= setUpCard;
			cardResource.cardEffect.CustomTextChanged -= setUpCard;
			mana.ManaChanged -= checkDisabled;
		}
		base.Dispose(disposing);
	}

}
